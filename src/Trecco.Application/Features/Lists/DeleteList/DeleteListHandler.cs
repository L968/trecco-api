using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Features.Lists.DeleteList;

internal sealed class DeleteListHandler(
    IBoardRepository boardRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DeleteListHandler> logger
) : IRequestHandler<DeleteListCommand, Result>
{
    public async Task<Result> Handle(DeleteListCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (!board.HasAccess(request.RequesterId))
        {
            return Result.Failure(BoardErrors.NotAuthorized);
        }

        board.RemoveList(request.ListId);

        await boardRepository.UpdateAsync(board, cancellationToken);

        await domainEventDispatcher.DispatchAsync(board, cancellationToken);

        logger.LogDebug("List {ListId} deleted from Board {BoardId}", request.ListId, board.Id);

        return Result.Success();
    }
}
