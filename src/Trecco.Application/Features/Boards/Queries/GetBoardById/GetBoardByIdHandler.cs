using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Queries.GetBoardById;

internal sealed class GetBoardByIdHandler(
    IBoardRepository boardRepository,
    ILogger<GetBoardByIdHandler> logger
) : IRequestHandler<GetBoardByIdQuery, Result<GetBoardByIdResponse>>
{
    public async Task<Result<GetBoardByIdResponse>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            return Result.Failure(BoardErrors.NotAuthorized);
        }

        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (!board.CanAccess(request.UserId.Value))
        {
            return Result.Failure(BoardErrors.NotAuthorized);
        }

        logger.LogDebug("Fetched board {@Board} for user {UserId}", board, request.UserId);

        var response = new GetBoardByIdResponse(
            Id: board.Id,
            Name: board.Name,
            Lists: board.Lists.Select(l => new BoardListDto(
                Id: l.Id,
                Name: l.Name,
                Position: l.Position,
                Cards: l.Cards.Select(c => new BoardCardDto(
                    Id: c.Id,
                    Title: c.Title,
                    Description: c.Description,
                    Position: c.Position
                )).ToList()
            )).ToList()
        );

        return Result.Success(response);
    }
}
