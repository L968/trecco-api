using MongoDB.Driver;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Queries.GetMyBoards;

namespace Trecco.Application.Infrastructure.Repositories;

internal sealed class BoardRepository : IBoardRepository
{
    private readonly IMongoCollection<Board> _boards;

    public BoardRepository(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
    }

    public async Task<IEnumerable<GetMyBoardsResponse>> GetBoardsByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        FilterDefinition<Board> filter = Builders<Board>.Filter.Or(
            Builders<Board>.Filter.Eq(b => b.OwnerUserId, userId),
            Builders<Board>.Filter.AnyIn("MemberIds", [userId.ToString()])
        );

        List<Board> boards = await _boards
            .Find(filter)
            .ToListAsync(cancellationToken);

        return boards.Select(b => new GetMyBoardsResponse(
            b.Id,
            b.Name,
            b.MemberIds.Count
        ));
    }

    public async Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _boards.Find(b => b.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task InsertAsync(Board board, CancellationToken cancellationToken) =>
        await _boards.InsertOneAsync(board, cancellationToken: cancellationToken);

    public async Task UpdateAsync(Board board, CancellationToken cancellationToken) =>
        await _boards.ReplaceOneAsync(
            b => b.Id == board.Id,
            board,
            cancellationToken: cancellationToken
        );

    public async Task DeleteAsync(Guid boardId, CancellationToken cancellationToken)
    {
        await _boards.DeleteOneAsync(
            b => b.Id == boardId,
            cancellationToken
        );
    }
}
