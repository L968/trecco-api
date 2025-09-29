namespace Trecco.Application.Domain.BoardActionLogs;

internal interface IBoardActionLogRepository
{
    Task AddAsync(BoardActionLog log, CancellationToken cancellationToken);
    Task<IEnumerable<BoardActionLog>> GetByBoardAsync(Guid boardId, CancellationToken cancellationToken);
}
