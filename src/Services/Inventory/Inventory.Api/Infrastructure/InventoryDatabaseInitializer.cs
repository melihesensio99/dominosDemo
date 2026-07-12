using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure;

public static class InventoryDatabaseInitializer
{
    public static async Task InitializeInventoryDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        await SeedStockAsync(dbContext, cancellationToken);
    }

    private static async Task SeedStockAsync(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.StockItems.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.StockItems.AddRange(
            new StockItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000002001"),
                ProductId = "p-100",
                Available = 25,
                Reserved = 0,
                ReorderLevel = 5,
                CreatedAt = DateTimeOffset.UtcNow,
            },
            new StockItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000002002"),
                ProductId = "p-200",
                Available = 12,
                Reserved = 0,
                ReorderLevel = 3,
                CreatedAt = DateTimeOffset.UtcNow,
            },
            new StockItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000002003"),
                ProductId = "p-300",
                Available = 5,
                Reserved = 0,
                ReorderLevel = 2,
                CreatedAt = DateTimeOffset.UtcNow,
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
