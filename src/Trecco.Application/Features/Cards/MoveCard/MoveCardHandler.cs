using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Cards.MoveCard;

internal sealed class MoveCardHandler(
    IBoardRepository boardRepository,
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

        board.MoveCard(request.CardId, request.TargetListId, request.TargetPosition);

        await boardRepository.UpdateAsync(board, cancellationToken);

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
