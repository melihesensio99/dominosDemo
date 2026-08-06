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
            ["Ac\u0131l\u0131 Kavurma"] = "Pizza sosu, mozzarella peyniri, kavurma, ye\u015fil biber ve ac\u0131 biber.",
            ["D\u00f6rt Peynirli"] = "Pizza sosu, mozzarella, cheddar, gorgonzola ve parmesan peyniri.",
            ["Kar\u0131\u015f\u0131k Lezzet"] = "Pizza sosu, mozzarella peyniri, sucuk, mantar, ye\u015fil biber ve k\u0131rm\u0131z\u0131 biber.",
            ["Margarita"] = "Pizza sosu, mozzarella peyniri ve taze fesle\u011fen.",
            ["Pepperoni"] = "Pizza sosu, mozzarella peyniri ve pepperoni.",
            ["Vejetaryen"] = "Pizza sosu, mozzarella peyniri, mantar, ye\u015fil biber, k\u0131rm\u0131z\u0131 biber, m\u0131s\u0131r, siyah zeytin ve so\u011fan.",
        };

        var pizzas = await dbContext.Products
            .Where(product => product.CategoryId == pizzaCategory.Id)
            .Include(product => product.OptionGroups)
                .ThenInclude(group => group.Options)
            .ToListAsync(cancellationToken);

        var descriptionsChanged = false;
        foreach (var pizza in pizzas)
        {
            pizza.InventoryTrackingType = InventoryTrackingType.Dough;
            pizza.InventoryKey = null;

            foreach (var option in pizza.OptionGroups
                         .Where(group => group.Name.Contains("Boyut", StringComparison.OrdinalIgnoreCase))
                         .SelectMany(group => group.Options))
            {
                option.InventoryKey = ResolveDoughStockKey(option.Name);
            }

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

        var directProducts = await dbContext.Products
            .Where(product => product.CategoryId != pizzaCategory.Id)
            .ToListAsync(cancellationToken);

        foreach (var product in directProducts)
        {
            product.InventoryTrackingType = InventoryTrackingType.Direct;
            product.InventoryKey = product.Id.ToString();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? ResolveDoughStockKey(string optionName)
    {
        var normalized = optionName.ToLowerInvariant();
        if (normalized.Contains("xl", StringComparison.Ordinal))
        {
            return "dough-xl";
        }

        if (normalized.Contains("k\u00fc\u00e7\u00fck", StringComparison.Ordinal))
        {
            return "dough-small";
        }

        if (normalized.Contains("b\u00fcy\u00fck", StringComparison.Ordinal))
        {
            return "dough-large";
        }

        if (normalized.Contains("küçük", StringComparison.Ordinal) || normalized.Contains("kucuk", StringComparison.Ordinal))
        {
            return "dough-small";
        }

        if (normalized.Contains("orta", StringComparison.Ordinal))
        {
            return "dough-medium";
        }

        return normalized.Contains("büyük", StringComparison.Ordinal) || normalized.Contains("buyuk", StringComparison.Ordinal)
            ? "dough-large"
            : null;
    }
}
