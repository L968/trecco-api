using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Trecco.Domain.Boards;

namespace Trecco.Infrastructure.Infrastructure.Hubs;

public class BoardHub(
    IBoardRepository boardRepository,
    ILogger<BoardHub> logger
) : Hub
{
    public override async Task OnConnectedAsync()
    {
        HttpContext? httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            logger.LogWarning("Connection aborted: HttpContext is null (ConnectionId: {ConnectionId})", Context.ConnectionId);
            Context.Abort();
            return;
        }

        string userIdHeader = httpContext.Request.Query["userId"];
        if (!Guid.TryParse(userIdHeader, out Guid userId))
        {
            logger.LogWarning("Connection aborted: Invalid x-user-id header (ConnectionId: {ConnectionId})", Context.ConnectionId);
            Context.Abort();
            return;
        }

        string boardIdQuery = httpContext.Request.Query["boardId"];
        if (!Guid.TryParse(boardIdQuery, out Guid boardId))
        {
            logger.LogWarning("Connection aborted: Invalid boardId query (ConnectionId: {ConnectionId}, UserId: {UserId})", Context.ConnectionId, userId);
            Context.Abort();
            return;
        }

        CancellationToken cancellationToken = Context.ConnectionAborted;
        Board? board = await boardRepository.GetByIdAsync(boardId, cancellationToken);
        if (board == null || !board.MemberIds.Contains(userId) && board.OwnerUserId != userId)
        {
            logger.LogWarning("Connection aborted: User {UserId} is not member or owner of board {BoardId}", userId, boardId);
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
        logger.LogDebug("User {UserId} connected to board {BoardId} (ConnectionId: {ConnectionId})", userId, boardId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogDebug("Connection disconnected (ConnectionId: {ConnectionId}, Exception: {Exception})", Context.ConnectionId, exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId.ToString());
        logger.LogDebug("Connection {ConnectionId} left board {BoardId}", Context.ConnectionId, boardId);
    }
}
