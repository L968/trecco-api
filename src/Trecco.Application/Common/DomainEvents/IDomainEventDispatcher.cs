using Trecco.Domain.DomainEvents;

namespace Trecco.Application.Common.DomainEvents;

internal interface IDomainEventDispatcher
{
    Task DispatchAsync(Entity entity, CancellationToken cancellationToken);
}
