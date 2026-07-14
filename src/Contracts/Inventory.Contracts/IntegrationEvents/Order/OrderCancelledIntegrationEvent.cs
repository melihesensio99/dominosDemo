namespace Inventory.Contracts.IntegrationEvents.Order;

public sealed record OrderCancelledIntegrationEvent(
    string OrderId,
    string CustomerId,
    string Status)
    : IntegrationEvent;
