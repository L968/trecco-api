using Microsoft.AspNetCore.SignalR;
using Trecco.Application.Common.Abstractions;

namespace Trecco.Infrastructure.Infrastructure.Hubs;

internal sealed class BoardHubNotifier : IBoardNotifier
{
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardHubNotifier(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task CardMovedAsync(Guid boardId, Guid cardId, Guid targetListId, int targetPosition, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(boardId.ToString())
            .SendAsync(
                "CardMoved",
                cardId,
                targetListId,
                targetPosition,
                cancellationToken
            );
    }

    public async Task BroadcastBoardLogAsync(Guid boardId, Guid logId, Guid userId, string details, DateTime timestamp, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(boardId.ToString())
            .SendAsync(
                "BoardLogged",
                logId,
                userId,
                details,
                timestamp,
                cancellationToken
            );
    }
}

