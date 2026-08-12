using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public static class InventoryDatabaseInitializer
{
    public static async Task InitializeInventoryDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        await dbContext.Database.MigrateDatabaseAsync(cancellationToken);
        await EnsureDoughStocksAsync(dbContext, cancellationToken);
    }

    private static async Task EnsureDoughStocksAsync(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        var definitions = new[]
        {
            new DoughStockDefinition("dough-small", "Küçük Boy Hamur", 40, 10),
            new DoughStockDefinition("dough-medium", "Orta Boy Hamur", 80, 15),
            new DoughStockDefinition("dough-large", "Büyük Boy Hamur", 50, 10),
            new DoughStockDefinition("dough-xl", "XL Hamur", 20, 5),
        };

        var existingKeys = await dbContext.StockItems
            .Where(item => definitions.Select(definition => definition.StockKey).Contains(item.StockKey))
            .Select(item => item.StockKey)
            .ToHashSetAsync(cancellationToken);

        var missingStocks = definitions
            .Where(definition => !existingKeys.Contains(definition.StockKey))
            .Select(definition => new StockItem
            {
                StockKey = definition.StockKey,
                DisplayName = definition.DisplayName,
                TrackingType = InventoryTrackingType.Dough,
                Available = definition.InitialStock,
                ReorderLevel = definition.ReorderLevel,
            });

        await dbContext.StockItems.AddRangeAsync(missingStocks, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record DoughStockDefinition(
        string StockKey,
        string DisplayName,
        int InitialStock,
        int ReorderLevel);
}
