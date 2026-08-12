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

    private static OutboxMessage CreateMessage(string type, DateTimeOffset occurredAt, object payload) =>
        new()
        {
            Type = type,
            OccurredAt = occurredAt,
            Payload = JsonSerializer.Serialize(payload, JsonOptions),
        };
}
