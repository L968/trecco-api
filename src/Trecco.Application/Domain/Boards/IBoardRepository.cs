using Trecco.Application.Features.Boards.Queries.GetMyBoards;

namespace Trecco.Application.Domain.Boards;

public interface IBoardRepository
{
    Task<IEnumerable<GetMyBoardsResponse>> GetBoardsByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task InsertAsync(Board board, CancellationToken cancellationToken);
    Task UpdateAsync(Board board, CancellationToken cancellationToken);
    Task DeleteAsync(Guid boardId, CancellationToken cancellationToken);
}
