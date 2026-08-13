namespace Inventory.Contracts.IntegrationEvents.Catalog;

public sealed record ProductDeletedIntegrationEvent(
    string ProductId,
    string ProductName,
    string TrackingType,
    string? StockKey) : IntegrationEvent;
