using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public sealed class EfStockRepository(InventoryDbContext dbContext) : IStockRepository
{
    public Task<StockItem?> GetByProductIdAsync(string productId, CancellationToken cancellationToken) =>
        dbContext.StockItems.FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

    public async Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.StockItems
            .OrderBy(x => x.ProductId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken)
    {
        await dbContext.StockItems.AddAsync(stockItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken)
    {
        dbContext.StockItems.Update(stockItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
