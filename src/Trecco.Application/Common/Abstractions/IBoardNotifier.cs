namespace Trecco.Application.Common.Abstractions;

public interface IBoardNotifier
{
    Task BroadcastCardMovedAsync(
        Guid boardId,
        Guid cardId,
        Guid targetListId,
        int targetPosition,
        CancellationToken cancellationToken
    );

    Task BroadcastBoardLoggedAsync(
        Guid boardId,
        Guid logId,
        Guid userId,
        string details,
        DateTime timestamp,
        CancellationToken cancellationToken
    );
}
