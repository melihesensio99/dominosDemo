namespace Order.Api.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
