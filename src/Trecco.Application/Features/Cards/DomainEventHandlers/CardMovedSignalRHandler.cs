using Microsoft.AspNetCore.SignalR;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Infrastructure.Hubs;

namespace Trecco.Application.Features.Cards.DomainEventHandlers;

internal sealed class CardMovedSignalRHandler : INotificationHandler<CardMovedDomainEvent>
{
    private readonly IHubContext<BoardHub> _hubContext;
    private readonly ILogger<CardMovedSignalRHandler> _logger;

    public CardMovedSignalRHandler(IHubContext<BoardHub> hubContext, ILogger<CardMovedSignalRHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(CardMovedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "DomainEvent: Sending SignalR notification to the board group {BoardId}. Card {CardId} moved to list {TargetListId} at position {TargetPosition}.",
            notification.BoardId, notification.CardId, notification.TargetListId, notification.TargetPosition
        );

        await _hubContext.Clients
            .Group(notification.BoardId.ToString())
            .SendAsync(
                "CardMoved",
                notification.CardId,
                notification.TargetListId,
                notification.TargetPosition,
                cancellationToken
            );
    }
}
