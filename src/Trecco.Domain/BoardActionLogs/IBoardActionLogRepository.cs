namespace Trecco.Domain.BoardActionLogs;

public interface IBoardActionLogRepository
{
    Task AddAsync(BoardActionLog log, CancellationToken cancellationToken = default);
    Task<IEnumerable<BoardActionLog>> GetByBoardAsync(Guid boardId, int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
}
