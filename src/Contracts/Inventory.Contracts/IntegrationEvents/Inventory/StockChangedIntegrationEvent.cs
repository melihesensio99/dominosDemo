namespace Inventory.Contracts.IntegrationEvents.Inventory;

public sealed record StockChangedIntegrationEvent(
    string ProductId,
    int Quantity,
    int Available,
    int Reserved,
    int ReorderLevel,
    StockOperationType OperationType)
    : IntegrationEvent;
