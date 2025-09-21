using MongoDB.Driver;
using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Infrastructure.Repositories;

internal sealed class BoardRepository : IBoardRepository
{
    private readonly IMongoCollection<Board> _boards;

    public BoardRepository(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken) =>
        await _boards.Find(b => b.Name == name).AnyAsync(cancellationToken);

    public async Task InsertAsync(Board board, CancellationToken cancellationToken) =>
        await _boards.InsertOneAsync(board, cancellationToken: cancellationToken);
}
