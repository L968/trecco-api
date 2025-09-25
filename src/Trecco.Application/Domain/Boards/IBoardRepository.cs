namespace Trecco.Application.Domain.Boards;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, Guid ownerUserId, CancellationToken cancellationToken);
    Task InsertAsync(Board board, CancellationToken cancellationToken);
    Task UpdateAsync(Board board, CancellationToken cancellationToken);
}
