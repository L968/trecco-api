using MongoDB.Driver;
using Trecco.Application.Domain.BoardActionLogs;

namespace Trecco.Application.Infrastructure.Repositories;

internal sealed class BoardActionLogRepository : IBoardActionLogRepository
{
    private readonly IMongoCollection<BoardActionLog> _logs;

    public BoardActionLogRepository(IMongoDatabase database)
    {
        _logs = database.GetCollection<BoardActionLog>("BoardActionLogs");
    }

    public async Task AddAsync(BoardActionLog log, CancellationToken cancellationToken)
    {
        await _logs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<BoardActionLog>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _logs
            .Find(l => l.BoardId == boardId)
            .SortByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }
}
