namespace Inventory.Contracts.IntegrationEvents.Order;

public sealed record OrderCreatedIntegrationEvent(
    string OrderId,
    string CustomerId,
    int ItemCount,
    string Status)
    : IntegrationEvent;
