namespace Inventory.Contracts.IntegrationEvents.Catalog;

public sealed record ProductCreatedIntegrationEvent(
    string ProductId,
    string ProductName,
    string TrackingType,
    string? StockKey,
    int InitialStock,
    int ReorderLevel) : IntegrationEvent;
