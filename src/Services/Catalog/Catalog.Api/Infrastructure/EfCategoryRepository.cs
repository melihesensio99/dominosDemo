using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure;

public sealed class EfCategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Categories
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        dbContext.Categories.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken) =>
        dbContext.Categories.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        dbContext.Categories.Update(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
