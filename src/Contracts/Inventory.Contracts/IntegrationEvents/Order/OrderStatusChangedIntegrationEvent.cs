namespace Inventory.Contracts.IntegrationEvents.Order;

public sealed record OrderStatusChangedIntegrationEvent(
    string OrderId,
    string CustomerId,
    string PreviousStatus,
    string Status)
    : IntegrationEvent;
