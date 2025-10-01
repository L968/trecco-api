namespace Trecco.Application.Common.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(Entity entity, CancellationToken cancellationToken);
}
