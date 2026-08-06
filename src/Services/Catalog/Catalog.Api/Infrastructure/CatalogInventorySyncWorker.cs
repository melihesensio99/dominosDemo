using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Infrastructure;

public sealed class CatalogInventorySyncWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<CatalogInventorySyncWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        var products = await dbContext.Products.AsNoTracking().ToListAsync(stoppingToken);

        foreach (var product in products)
        {
            await publishEndpoint.Publish(new ProductCreatedIntegrationEvent(
                product.Id.ToString(),
                product.Name,
                product.InventoryTrackingType.ToString().ToLowerInvariant(),
                product.InventoryKey,
                product.Stock,
                ResolveReorderLevel(product)), stoppingToken);
        }

        logger.LogInformation("Synchronized {ProductCount} catalog products with inventory.", products.Count);
    }

    private static int ResolveReorderLevel(Product product) =>
        product.InventoryTrackingType == InventoryTrackingType.Direct
            ? Math.Max(1, Math.Min(10, product.Stock / 5))
            : 0;
}
