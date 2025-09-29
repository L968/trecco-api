using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

internal sealed class GetMyBoardsHandler(
    IBoardRepository boardRepository,
    ILogger<GetMyBoardsHandler> logger
) : IRequestHandler<GetMyBoardsQuery, IEnumerable<GetMyBoardsResponse>>
{
    public async Task<IEnumerable<GetMyBoardsResponse>> Handle(GetMyBoardsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<GetMyBoardsResponse> boards = await boardRepository.GetBoardsByUserAsync(request.OwnerUserId, cancellationToken);

        logger.LogDebug("Fetched {Count} boards for owner {OwnerUserId}", boards.Count(), request.OwnerUserId);

        return boards;
    }
}
