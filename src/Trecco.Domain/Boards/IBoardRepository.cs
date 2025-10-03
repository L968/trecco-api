namespace Trecco.Domain.Boards;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Board>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task InsertAsync(Board board, CancellationToken cancellationToken);
    Task UpdateAsync(Board board, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
