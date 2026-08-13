using System.Text.Json;
using Inventory.Contracts.IntegrationEvents.Catalog;

namespace Catalog.Api.Infrastructure.Outbox;

public static class CatalogOutboxMessageFactory
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static OutboxMessage CreateProductCreated(Product product, int reorderLevel) =>
        CreateMessage(
            "catalog.product-created",
            product.CreatedAt,
            new ProductCreatedIntegrationEvent(
                product.Id.ToString(),
                product.Name,
                product.InventoryTrackingType.ToString().ToLowerInvariant(),
                product.InventoryKey,
                product.Stock,
                reorderLevel));

    public static OutboxMessage CreateProductUpdated(Product product) =>
        CreateMessage(
            "catalog.product-updated",
            product.UpdatedAt ?? DateTimeOffset.UtcNow,
            new ProductUpdatedIntegrationEvent(
                product.Id.ToString(),
                product.Name,
                product.InventoryTrackingType.ToString().ToLowerInvariant(),
                product.InventoryKey,
                product.Stock,
                product.ReorderLevel,
                product.Price,
                product.IsActive));

    public static OutboxMessage CreateProductDeleted(Product product) =>
        CreateMessage(
            "catalog.product-deleted",
            DateTimeOffset.UtcNow,
            new ProductDeletedIntegrationEvent(
                product.Id.ToString(),
                product.Name,
                product.InventoryTrackingType.ToString().ToLowerInvariant(),
                product.InventoryKey));

    private static OutboxMessage CreateMessage(string type, DateTimeOffset occurredAt, object payload) =>
        new()
        {
            Type = type,
            OccurredAt = occurredAt,
            Payload = JsonSerializer.Serialize(payload, JsonOptions),
        };
}
