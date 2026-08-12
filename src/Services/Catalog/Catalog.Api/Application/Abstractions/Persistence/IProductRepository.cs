namespace Catalog.Api.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task AddWithInventorySyncAsync(Product product, int reorderLevel, CancellationToken cancellationToken);

    Task UpdateAsync(Product product, CancellationToken cancellationToken);

    Task DeleteAsync(Product product, CancellationToken cancellationToken);
}
