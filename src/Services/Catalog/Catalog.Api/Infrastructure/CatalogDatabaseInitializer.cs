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

        if (missingCategories.Length > 0)
        {
            await dbContext.Categories.AddRangeAsync(missingCategories, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var pizzaCategory = await dbContext.Categories
            .SingleOrDefaultAsync(category => category.Slug == "pizzalar", cancellationToken);

        if (pizzaCategory is null)
        {
            return;
        }

        var pizzaDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Ac\u0131l\u0131 Kavurma"] = "Pizza sosu, mozzarella peyniri, kavurma, yesil biber ve aci biber.",
            ["D\u00f6rt Peynirli"] = "Pizza sosu, mozzarella, cheddar, gorgonzola ve parmesan peyniri.",
            ["Kar\u0131\u015f\u0131k Lezzet"] = "Pizza sosu, mozzarella peyniri, sucuk, mantar, yesil biber ve kirmizi biber.",
            ["Margarita"] = "Pizza sosu, mozzarella peyniri ve taze feslegen.",
            ["Pepperoni"] = "Pizza sosu, mozzarella peyniri ve pepperoni.",
            ["Vejetaryen"] = "Pizza sosu, mozzarella peyniri, mantar, yesil biber, kirmizi biber, misir, siyah zeytin ve sogan.",
        };

        var pizzas = await dbContext.Products
            .Where(product => product.CategoryId == pizzaCategory.Id)
            .ToListAsync(cancellationToken);

        var descriptionsChanged = false;
        foreach (var pizza in pizzas)
        {
            if (!pizzaDescriptions.TryGetValue(pizza.Name, out var description) || pizza.Description == description)
            {
                continue;
            }

            pizza.Description = description;
            pizza.UpdatedAt = DateTimeOffset.UtcNow;
            descriptionsChanged = true;
        }

        if (descriptionsChanged)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
