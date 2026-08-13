using Microsoft.EntityFrameworkCore;
using Catalog.Api.Infrastructure.Outbox;

namespace Catalog.Api.Infrastructure.Persistence;

public sealed class EfProductRepository(CatalogDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.OptionGroups)
                .ThenInclude(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Products
            .Include(x => x.Category)
            .Include(x => x.OptionGroups)
                .ThenInclude(x => x.Options)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        dbContext.Products.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken) =>
        dbContext.Products.AnyAsync(x => x.CategoryId == categoryId, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddWithInventorySyncAsync(Product product, int reorderLevel, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
        dbContext.OutboxMessages.Add(CatalogOutboxMessageFactory.CreateProductCreated(product, reorderLevel));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWithInventorySyncAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        dbContext.OutboxMessages.Add(CatalogOutboxMessageFactory.CreateProductUpdated(product));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWithInventorySyncAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Remove(product);
        dbContext.OutboxMessages.Add(CatalogOutboxMessageFactory.CreateProductDeleted(product));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
