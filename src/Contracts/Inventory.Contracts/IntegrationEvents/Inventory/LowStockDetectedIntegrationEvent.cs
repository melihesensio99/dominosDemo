namespace Inventory.Contracts.IntegrationEvents.Inventory;

public sealed record LowStockDetectedIntegrationEvent(
    string StockKey,
    string DisplayName,
    int Available,
    int ReorderLevel) : IntegrationEvent;
