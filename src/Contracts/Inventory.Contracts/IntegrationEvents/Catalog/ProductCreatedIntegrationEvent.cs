namespace Inventory.Contracts.IntegrationEvents.Catalog;

public sealed record ProductCreatedIntegrationEvent(
    string ProductId,
    int InitialStock,
    int ReorderLevel) : IntegrationEvent;
