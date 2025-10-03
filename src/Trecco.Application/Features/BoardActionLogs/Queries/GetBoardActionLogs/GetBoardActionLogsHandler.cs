using Trecco.Application.Common;
using Trecco.Domain.BoardActionLogs;

namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

internal sealed class GetBoardActionLogsHandler(
    IBoardActionLogRepository logRepository,
    IBoardRepository boardRepository,
    ILogger<GetBoardActionLogsHandler> logger
) : IRequestHandler<GetBoardActionLogsQuery, Result<PaginatedList<GetBoardActionLogsResponse>>>
{
    public async Task<Result<PaginatedList<GetBoardActionLogsResponse>>> Handle(GetBoardActionLogsQuery request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (!board.HasAccess(request.UserId))
        {
            return Result.Failure(BoardErrors.NotAuthorized);
        }

        IEnumerable<BoardActionLog> logs = await logRepository.GetByBoardAsync(
            request.BoardId,
            request.Page,
            request.PageSize,
            request.SearchTerm,
            cancellationToken
        );

        IEnumerable<GetBoardActionLogsResponse> responseItems = logs.Select(log => new GetBoardActionLogsResponse(
            log.Id,
            log.UserId,
            log.Details,
            log.Timestamp
        ));

        var paginatedResponse = new PaginatedList<GetBoardActionLogsResponse>(
            request.Page,
            request.PageSize,
            null,
            responseItems
        );

        logger.LogDebug("Fetched {Count} action logs for board {BoardId} (Page {Page})", logs.Count(), request.BoardId, request.Page);

        return Result.Success(paginatedResponse);
    }
}
