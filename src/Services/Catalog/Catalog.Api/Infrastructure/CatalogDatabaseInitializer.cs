using BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure;

public static class CatalogDatabaseInitializer
{
    public static async Task InitializeCatalogDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        await dbContext.Database.MigrateOrEnsureCreatedAsync(cancellationToken);

        var categories = new[]
        {
            new Category
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000201"),
                Name = "Pizzalar",
                Slug = "pizzalar",
            },
            new Category
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000202"),
                Name = "Patatesler",
                Slug = "patatesler",
            },
            new Category
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000203"),
                Name = "Soslar",
                Slug = "soslar",
            },
            new Category
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000204"),
                Name = "İçecekler",
                Slug = "icecekler",
            },
            new Category
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000205"),
                Name = "Tatlılar",
                Slug = "tatlilar",
            },
        };

        var existingSlugs = await dbContext.Categories
            .Select(category => category.Slug)
            .ToHashSetAsync(cancellationToken);

        var missingCategories = categories
            .Where(category => !existingSlugs.Contains(category.Slug))
            .ToArray();

        if (missingCategories.Length == 0)
        {
            return;
        }

        await dbContext.Categories.AddRangeAsync(missingCategories, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
