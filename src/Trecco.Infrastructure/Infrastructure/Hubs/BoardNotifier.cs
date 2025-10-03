using Microsoft.AspNetCore.SignalR;
using Trecco.Application.Common.Abstractions;

namespace Trecco.Infrastructure.Infrastructure.Hubs;

internal sealed class BoardHubNotifier(
    IHubContext<BoardHub> hubContext
) : IBoardNotifier
{
    public async Task BroadcastCardMovedAsync(Guid boardId, Guid cardId, Guid targetListId, int targetPosition, CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .Group(boardId.ToString())
            .SendAsync(
                "CardMoved",
                cardId,
                targetListId,
                targetPosition,
                cancellationToken
            );
    }

    public async Task BroadcastBoardLoggedAsync(Guid boardId, Guid logId, Guid userId, string details, DateTime timestamp, CancellationToken cancellationToken)
    {
        await hubContext.Clients
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
