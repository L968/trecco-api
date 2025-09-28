using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Cards.DeleteCard;

internal sealed class DeleteCardHandler(
    IBoardRepository boardRepository,
    ILogger<DeleteCardHandler> logger
) : IRequestHandler<DeleteCardCommand, Result>
{
    public async Task<Result> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
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

        board.DeleteCard(request.CardId);

        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogDebug("Card {CardId} deleted from Board {BoardId}", request.CardId, board.Id);

        return Result.Success();
    }
}
