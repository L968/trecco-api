using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Features.Lists.CreateList;

internal sealed class CreateListHandler(
    IBoardRepository boardRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<CreateListHandler> logger
) : IRequestHandler<CreateListCommand, Result>
{
    public async Task<Result> Handle(CreateListCommand request, CancellationToken cancellationToken)
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

        List list = board.AddList(request.Name);

        await boardRepository.UpdateAsync(board, cancellationToken);

        await domainEventDispatcher.DispatchAsync(board, cancellationToken);

        logger.LogDebug("List {ListId} created in Board {BoardId}", list.Id, board.Id);

        return Result.Success();
    }
}
