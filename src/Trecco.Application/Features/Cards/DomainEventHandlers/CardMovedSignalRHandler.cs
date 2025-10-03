using Trecco.Application.Common.Abstractions;
using Trecco.Domain.Cards;

namespace Trecco.Application.Features.Cards.DomainEventHandlers;

internal sealed class CardMovedSignalRHandler(
    IBoardNotifier notifier,
    ILogger<CardMovedSignalRHandler> logger
    ) : INotificationHandler<CardMovedDomainEvent>
{
    public async Task Handle(CardMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "DomainEvent: Sending SignalR notification to the board group {BoardId}. Card {CardId} moved to list {TargetListId} at position {TargetPosition}.",
            notification.BoardId, notification.CardId, notification.TargetListId, notification.TargetPosition
        );

        await notifier.BroadcastCardMovedAsync(
            notification.BoardId,
            notification.CardId,
            notification.TargetListId,
            notification.TargetPosition,
            cancellationToken
        );
    }
}
