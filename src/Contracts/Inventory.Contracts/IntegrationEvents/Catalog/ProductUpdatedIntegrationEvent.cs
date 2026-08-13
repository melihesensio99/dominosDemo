namespace Inventory.Contracts.IntegrationEvents.Catalog;

public sealed record ProductUpdatedIntegrationEvent(
    string ProductId,
    string ProductName,
    string TrackingType,
    string? StockKey,
    int Stock,
    int ReorderLevel,
    decimal Price,
    bool IsActive) : IntegrationEvent;
