using Trecco.Application.Features.Boards.Queries.GetMyBoards;

namespace Trecco.Application.Domain.Boards;

public interface IBoardRepository
{
    Task<IEnumerable<GetMyBoardsResponse>> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken);
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task InsertAsync(Board board, CancellationToken cancellationToken);
    Task UpdateAsync(Board board, CancellationToken cancellationToken);
}
