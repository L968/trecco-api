using Trecco.Application.Domain.Boards;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Features.Cards.CreateCard;

internal sealed class CreateCardHandler(
    IBoardRepository boardRepository,
    ILogger<CreateCardHandler> logger
) : IRequestHandler<CreateCardCommand, Result>
{
    public async Task<Result> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        List? list = board.Lists.FirstOrDefault(l => l.Id == request.ListId);

        if (list is null)
        {
            return Result.Failure(ListErrors.NotFound(request.ListId));
        }

        Card card = list.AddCard(request.Title, request.Description);

        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogDebug(
            "Card {CardId} created in List {ListId} on Board {BoardId}",
            card.Id, list.Id, board.Id
        );

        return Result.Success();
    }
}
