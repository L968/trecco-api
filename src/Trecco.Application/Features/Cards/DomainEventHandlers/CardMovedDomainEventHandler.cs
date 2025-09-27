using Microsoft.AspNetCore.SignalR;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Infrastructure.Hubs;

namespace Trecco.Application.Features.Cards.DomainEventHandlers;

internal sealed class CardMovedEventHandler : INotificationHandler<CardMovedDomainEvent>
{
    private readonly IHubContext<BoardHub> _hubContext;

    public CardMovedEventHandler(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(CardMovedDomainEvent notification, CancellationToken cancellationToken)
    {
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
