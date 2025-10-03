using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application.Features.Cards.MoveCard;

internal sealed class MoveCardHandler(
    IBoardRepository boardRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<MoveCardHandler> logger
) : IRequestHandler<MoveCardCommand, Result>
{
    public async Task<Result> Handle(MoveCardCommand request, CancellationToken cancellationToken)
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

        Result moveResult = board.MoveCard(request.CardId, request.TargetListId, request.TargetPosition);
        if (moveResult.IsFailure)
        {
            return moveResult;
        }

        await boardRepository.UpdateAsync(board, cancellationToken);

        await domainEventDispatcher.DispatchAsync(board, cancellationToken);

        logger.LogDebug(
            "Card {CardId} successfully moved to list {TargetListId} at position {TargetPosition} in board {BoardId}",
            request.CardId,
            request.TargetListId,
            request.TargetPosition,
            request.BoardId
        );

        return Result.Success();
    }
}
