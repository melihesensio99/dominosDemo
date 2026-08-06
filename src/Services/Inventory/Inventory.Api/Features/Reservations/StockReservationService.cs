using System.Text.Json;
using Inventory.Contracts.IntegrationEvents.Inventory;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Features.Reservations;

public sealed class StockReservationService(
    InventoryDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    ILogger<StockReservationService> logger)
{
    public async Task<StockReservationResult> ReserveAsync(
        string orderId,
        IEnumerable<ReservedStockItem> requestedItems,
        CancellationToken cancellationToken)
    {
        var items = requestedItems
            .GroupBy(item => item.StockKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ReservedStockItem(group.Key, group.Sum(item => item.Quantity)))
            .OrderBy(item => item.StockKey, StringComparer.Ordinal)
            .ToArray();

        if (string.IsNullOrWhiteSpace(orderId) || items.Length == 0 || items.Any(item => item.Quantity <= 0))
        {
            return StockReservationResult.Failed("inventory.invalid_reservation", "Order id and positive stock quantities are required.");
        }

        var existing = await dbContext.StockReservations
            .AsNoTracking()
            .SingleOrDefaultAsync(reservation => reservation.OrderId == orderId, cancellationToken);
        if (existing is not null)
        {
            return existing.Status == StockReservationStatus.Active
                ? StockReservationResult.Succeeded()
                : StockReservationResult.Failed("inventory.reservation_closed", "The stock reservation is already closed.");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var changed = new List<(StockItem Item, int Quantity)>();

        foreach (var requestedItem in items)
        {
            var updated = await dbContext.StockItems
                .Where(item => item.StockKey == requestedItem.StockKey
                    && item.IsActive
                    && item.Available >= requestedItem.Quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.Available, item => item.Available - requestedItem.Quantity)
                    .SetProperty(item => item.Reserved, item => item.Reserved + requestedItem.Quantity)
                    .SetProperty(item => item.UpdatedAt, DateTimeOffset.UtcNow), cancellationToken);

            if (updated == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return StockReservationResult.Failed(
                    "inventory.insufficient_stock",
                    $"Not enough stock is available for '{requestedItem.StockKey}'.");
            }

            var stock = await dbContext.StockItems
                .SingleAsync(item => item.StockKey == requestedItem.StockKey, cancellationToken);
            changed.Add((stock, requestedItem.Quantity));
        }

        var lowStocks = changed
            .Select(change => change.Item)
            .Where(item => item.Available <= item.ReorderLevel && !item.LowStockNotified)
            .ToArray();
        foreach (var stock in lowStocks)
        {
            stock.LowStockNotified = true;
        }

        await dbContext.StockReservations.AddAsync(new StockReservation
        {
            OrderId = orderId,
            ItemsJson = JsonSerializer.Serialize(items),
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await PublishStockChangesAsync(changed, StockOperationType.Reserved, lowStocks, cancellationToken);
        return StockReservationResult.Succeeded();
    }

    public Task<StockReservationResult> ReleaseAsync(string orderId, CancellationToken cancellationToken) =>
        CloseAsync(orderId, StockReservationStatus.Released, cancellationToken);

    public Task<StockReservationResult> CommitAsync(string orderId, CancellationToken cancellationToken) =>
        CloseAsync(orderId, StockReservationStatus.Committed, cancellationToken);

    private async Task<StockReservationResult> CloseAsync(
        string orderId,
        StockReservationStatus targetStatus,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var reservation = await dbContext.StockReservations
            .SingleOrDefaultAsync(item => item.OrderId == orderId, cancellationToken);
        if (reservation is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return StockReservationResult.Failed("inventory.reservation_not_found", "Stock reservation was not found.");
        }

        if (reservation.Status == targetStatus)
        {
            await transaction.RollbackAsync(cancellationToken);
            return StockReservationResult.Succeeded();
        }

        if (reservation.Status != StockReservationStatus.Active)
        {
            await transaction.RollbackAsync(cancellationToken);
            return StockReservationResult.Failed("inventory.reservation_closed", "The stock reservation is already closed.");
        }

        var items = JsonSerializer.Deserialize<ReservedStockItem[]>(reservation.ItemsJson) ?? [];
        var changed = new List<(StockItem Item, int Quantity)>();
        foreach (var reservedItem in items.OrderBy(item => item.StockKey, StringComparer.Ordinal))
        {
            var query = dbContext.StockItems.Where(item =>
                item.StockKey == reservedItem.StockKey && item.Reserved >= reservedItem.Quantity);
            var updated = targetStatus == StockReservationStatus.Released
                ? await query.ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.Available, item => item.Available + reservedItem.Quantity)
                    .SetProperty(item => item.Reserved, item => item.Reserved - reservedItem.Quantity)
                    .SetProperty(item => item.UpdatedAt, DateTimeOffset.UtcNow), cancellationToken)
                : await query.ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.Reserved, item => item.Reserved - reservedItem.Quantity)
                    .SetProperty(item => item.UpdatedAt, DateTimeOffset.UtcNow), cancellationToken);

            if (updated == 0)
            {
                await transaction.RollbackAsync(cancellationToken);
                return StockReservationResult.Failed("inventory.reservation_inconsistent", "Reserved stock could not be closed safely.");
            }

            var stock = await dbContext.StockItems
                .SingleAsync(item => item.StockKey == reservedItem.StockKey, cancellationToken);
            if (targetStatus == StockReservationStatus.Released && stock.Available > stock.ReorderLevel)
            {
                stock.LowStockNotified = false;
            }

            changed.Add((stock, reservedItem.Quantity));
        }

        reservation.Status = targetStatus;
        reservation.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var operation = targetStatus == StockReservationStatus.Released
            ? StockOperationType.Released
            : StockOperationType.Committed;
        await PublishStockChangesAsync(changed, operation, [], cancellationToken);
        return StockReservationResult.Succeeded();
    }

    private async Task PublishStockChangesAsync(
        IEnumerable<(StockItem Item, int Quantity)> changes,
        StockOperationType operation,
        IReadOnlyCollection<StockItem> lowStocks,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var (item, quantity) in changes)
            {
                await publishEndpoint.Publish(new StockChangedIntegrationEvent(
                    item.StockKey,
                    quantity,
                    item.Available,
                    item.Reserved,
                    item.ReorderLevel,
                    operation), cancellationToken);
            }

            foreach (var stock in lowStocks)
            {
                await publishEndpoint.Publish(new LowStockDetectedIntegrationEvent(
                    stock.StockKey,
                    stock.DisplayName,
                    stock.Available,
                    stock.ReorderLevel), cancellationToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Stock was changed, but its integration events could not be published.");
        }
    }
}

public sealed record StockReservationResult(bool Success, string? ErrorCode, string? Message)
{
    public static StockReservationResult Succeeded() => new(true, null, null);

    public static StockReservationResult Failed(string code, string message) => new(false, code, message);
}
