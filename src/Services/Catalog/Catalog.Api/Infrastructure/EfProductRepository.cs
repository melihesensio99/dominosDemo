using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure;

public sealed class EfProductRepository(CatalogDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Products
            .Include(x => x.Category)
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

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
