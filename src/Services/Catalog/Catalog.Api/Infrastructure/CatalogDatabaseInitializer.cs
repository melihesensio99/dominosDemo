using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure;

public static class CatalogDatabaseInitializer
{
    public static async Task InitializeCatalogDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        await SeedCategoriesAsync(dbContext, cancellationToken);
        await SeedProductsAsync(dbContext, cancellationToken);
    }

    private static async Task SeedCategoriesAsync(CatalogDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Categories.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Categories.AddRange(
            new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000101"), Name = "General", Slug = "general" },
            new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000102"), Name = "Enterprise", Slug = "enterprise" },
            new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000103"), Name = "Starter", Slug = "starter" });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedProductsAsync(CatalogDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var starterCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000103");
        var enterpriseCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000102");

        dbContext.Products.AddRange(
            new Product
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000001001"),
                Name = "Starter Box",
                Description = "Basic starter package for small teams.",
                Price = 100m,
                Stock = 25,
                CategoryId = starterCategoryId,
                CreatedAt = DateTimeOffset.UtcNow,
            },
            new Product
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000001002"),
                Name = "Enterprise Box",
                Description = "Large package for enterprise customers.",
                Price = 500m,
                Stock = 5,
                CategoryId = enterpriseCategoryId,
                CreatedAt = DateTimeOffset.UtcNow,
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
