namespace Inventory.Contracts.IntegrationEvents;

public abstract record IntegrationEvent(
    Guid EventId,
    DateTimeOffset OccurredAt)
{
    protected IntegrationEvent()
        : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}
