using MongoDB.Driver;
using Trecco.Domain.BoardActionLogs;

namespace Trecco.Infrastructure.Infrastructure.Repositories;

internal sealed class BoardActionLogRepository : IBoardActionLogRepository
{
    private readonly IMongoCollection<BoardActionLog> _boardLogs;

    public BoardActionLogRepository(IMongoDatabase database)
    {
        _boardLogs = database.GetCollection<BoardActionLog>("BoardActionLogs");
    }

    public async Task AddAsync(BoardActionLog log, CancellationToken cancellationToken = default)
    {
        await _boardLogs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<BoardActionLog>> GetByBoardAsync(
        Guid boardId,
        int page,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default
        )
    {
        FilterDefinition<BoardActionLog> filter = Builders<BoardActionLog>.Filter.Eq(l => l.BoardId, boardId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            FilterDefinition<BoardActionLog> searchFilter = Builders<BoardActionLog>.Filter.Regex(
                l => l.Details,
                new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")
            );
            filter = Builders<BoardActionLog>.Filter.And(filter, searchFilter);
        }

        return await _boardLogs
            .Find(filter)
            .SortByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }
}
