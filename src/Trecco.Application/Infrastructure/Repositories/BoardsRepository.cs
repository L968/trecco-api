using MongoDB.Driver;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Features.Boards.Queries.GetBoardsByOwner;

namespace Trecco.Application.Infrastructure.Repositories;

internal sealed class BoardRepository : IBoardRepository
{
    private readonly IMongoCollection<Board> _boards;

    public BoardRepository(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
    }

    public async Task<IEnumerable<GetBoardsByOwnerResponse>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken) =>
        await _boards
            .Find(b => b.OwnerUserId == ownerUserId)
            .Project(b => new GetBoardsByOwnerResponse(
                b.Id,
                b.Name,
                b.OwnerUserId
            ))
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, Guid ownerUserId, CancellationToken cancellationToken) =>
        await _boards.Find(b => b.Name == name && b.OwnerUserId == ownerUserId)
                     .AnyAsync(cancellationToken);

    public async Task InsertAsync(Board board, CancellationToken cancellationToken) =>
        await _boards.InsertOneAsync(board, cancellationToken: cancellationToken);

    public async Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _boards.Find(b => b.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task UpdateAsync(Board board, CancellationToken cancellationToken) =>
        await _boards.ReplaceOneAsync(b => b.Id == board.Id, board, cancellationToken: cancellationToken);
}
