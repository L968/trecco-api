using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application.Common.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(
        this IMediator mediator,
        Entity entity,
        CancellationToken cancellationToken)
    {
        foreach (INotification domainEvent in entity.DomainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        entity.ClearDomainEvents();
    }
}
