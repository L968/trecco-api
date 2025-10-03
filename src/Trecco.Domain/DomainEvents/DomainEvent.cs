using MediatR;

namespace Trecco.Domain.DomainEvents;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
