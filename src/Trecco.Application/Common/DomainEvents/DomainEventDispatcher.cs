using Trecco.Domain.DomainEvents;

namespace Trecco.Application.Common.DomainEvents;

internal sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task DispatchAsync(Entity entity, CancellationToken cancellationToken)
    {
        foreach (INotification domainEvent in entity.DomainEvents)
        {
            try
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
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
