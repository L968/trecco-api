namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

internal sealed class GetMyBoardsHandler(
    IBoardRepository boardRepository,
    ILogger<GetMyBoardsHandler> logger
) : IRequestHandler<GetMyBoardsQuery, IEnumerable<GetMyBoardsResponse>>
{
    public async Task<IEnumerable<GetMyBoardsResponse>> Handle(GetMyBoardsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Board> boards = await boardRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        logger.LogDebug("Fetched {Count} boards for owner {OwnerUserId}", boards.Count(), request.UserId);

        return boards.Select(b => new GetMyBoardsResponse(
            b.Id,
            b.Name,
            b.MemberIds.Count
        ));
    }
}
