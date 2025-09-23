namespace Trecco.Application.Domain.Boards;

public interface IBoardRepository
{
    Task<bool> ExistsByNameAsync(string name, Guid ownerUserId, CancellationToken cancellationToken);
    Task InsertAsync(Board board, CancellationToken cancellationToken);
}
