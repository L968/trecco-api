using Microsoft.AspNetCore.SignalR;

namespace Trecco.Application.Infrastructure.Hubs;

public class BoardHub : Hub
{
    public async Task JoinBoard(Guid boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
    }

    public async Task LeaveBoard(Guid boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId.ToString());
    }

    public async Task CardMoved(Guid boardId, Guid cardId, Guid listId, int position)
    {
        await Clients
            .Group(boardId.ToString())
            .SendAsync("CardMoved", cardId, listId, position);
    }
}
