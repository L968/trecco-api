using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Lists.UpdateListName;

internal sealed class UpdateListNameHandler(
    IBoardRepository boardRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<UpdateListNameHandler> logger
) : IRequestHandler<UpdateListNameCommand, Result>
{
    public async Task<Result> Handle(UpdateListNameCommand request, CancellationToken cancellationToken)
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

        board.RenameList(request.ListId, request.Name);

        await boardRepository.UpdateAsync(board, cancellationToken);

        await domainEventDispatcher.DispatchAsync(board, cancellationToken);

        logger.LogInformation("List {ListId} renamed to {Name} in Board {BoardId}", request.ListId, request.Name, board.Id);

        return Result.Success();
    }
}
