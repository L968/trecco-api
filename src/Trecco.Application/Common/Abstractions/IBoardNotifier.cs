namespace Trecco.Application.Common.Abstractions;

public interface IBoardNotifier
{
    Task CardMovedAsync(
        Guid boardId,
        Guid cardId,
        Guid targetListId,
        int targetPosition,
        CancellationToken cancellationToken
    );

    Task BroadcastBoardLogAsync(
        Guid boardId,
        Guid logId,
        Guid userId,
        string details,
        DateTime timestamp,
        CancellationToken cancellationToken
    );
}
