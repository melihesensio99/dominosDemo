namespace Catalog.Api.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);

    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task AddAsync(Category category, CancellationToken cancellationToken);

    Task UpdateAsync(Category category, CancellationToken cancellationToken);

    Task DeleteAsync(Category category, CancellationToken cancellationToken);
}
