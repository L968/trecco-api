using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Queries.GetBoardsByOwner;

internal sealed class GetBoardsByOwnerHandler(
    IBoardRepository boardRepository,
    ILogger<GetBoardsByOwnerHandler> logger
) : IRequestHandler<GetBoardsByOwnerQuery, IEnumerable<GetBoardsByOwnerResponse>>
{
    public async Task<IEnumerable<GetBoardsByOwnerResponse>> Handle(GetBoardsByOwnerQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<GetBoardsByOwnerResponse> boards = await boardRepository.GetByOwnerAsync(request.OwnerUserId, cancellationToken);

        logger.LogDebug("Fetched {Count} boards for owner {OwnerUserId}", boards.Count(), request.OwnerUserId);

        return boards;
    }
}
