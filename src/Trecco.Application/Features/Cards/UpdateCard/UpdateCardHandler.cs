using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Cards;

namespace Trecco.Application.Features.Cards.UpdateCard;

internal sealed class UpdateCardHandler(
    IBoardRepository boardRepository,
    ILogger<UpdateCardHandler> logger
) : IRequestHandler<UpdateCardCommand, Result>
{
    public async Task<Result> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
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

        Card? card = board.GetCardById(request.CardId);
        if (card is null)
        {
            return Result.Failure(CardErrors.NotFound(request.CardId));
        }

        card.UpdateTitle(request.Title);
        card.UpdateDescription(request.Description);

        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogDebug(
            "Card {CardId} updated in Board {BoardId}",
            card.Id,
            board.Id
        );

        return Result.Success();
    }
}
