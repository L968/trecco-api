using Trecco.Domain.DomainEvents;

namespace Trecco.Application.Common.DomainEvents;

internal sealed class DomainEventDispatcher(
    IMediator mediator,
    ILogger<DomainEventDispatcher> logger
) : IDomainEventDispatcher
{
    public async Task DispatchAsync(Entity entity, CancellationToken cancellationToken)
    {
        foreach (INotification domainEvent in entity.DomainEvents)
        {
            try
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error processing DomainEvent {EventType} for entity {EntityType} {@Entity}",
                    domainEvent.GetType().Name,
                    entity.GetType().Name,
                    entity
                );
            }
        }

        entity.ClearDomainEvents();
    }
}
