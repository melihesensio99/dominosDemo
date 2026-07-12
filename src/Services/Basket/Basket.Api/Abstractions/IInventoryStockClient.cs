namespace Basket.Api.Abstractions;

public interface IInventoryStockClient
{
    Task<Result<StockSnapshot>> GetStockAsync(string productId, CancellationToken cancellationToken);

    Task<bool> IsReadyAsync(CancellationToken cancellationToken);
}

public sealed record StockSnapshot(string ProductId, int Available, int Reserved, int ReorderLevel)
{
    public bool CanFit(int quantity) => quantity <= Available;
}
