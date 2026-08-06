namespace Inventory.Api.Abstractions;

public interface IStockRepository
{
    Task<StockItem?> GetByStockKeyAsync(string stockKey, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(StockItem stockItem, CancellationToken cancellationToken);

    Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken);
}
