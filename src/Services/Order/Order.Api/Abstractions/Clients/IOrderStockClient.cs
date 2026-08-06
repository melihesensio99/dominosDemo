namespace Order.Api.Abstractions;

public interface IOrderStockClient
{
    Task<OrderStockResult> ReserveAsync(
        string orderId,
        IReadOnlyCollection<OrderStockRequirement> requirements,
        CancellationToken cancellationToken);

    Task<OrderStockResult> ReleaseAsync(string orderId, CancellationToken cancellationToken);
}

public sealed record OrderStockRequirement(string StockKey, int Quantity);

public sealed record OrderStockResult(bool Success, string? ErrorCode, string? Message);
