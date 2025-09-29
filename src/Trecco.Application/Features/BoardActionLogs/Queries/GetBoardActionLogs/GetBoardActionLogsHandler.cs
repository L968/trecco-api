using Trecco.Application.Domain.BoardActionLogs;
using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

internal sealed class GetBoardActionLogsHandler(
    IBoardActionLogRepository logRepository,
    IBoardRepository boardRepository,
    ILogger<GetBoardActionLogsHandler> logger
) : IRequestHandler<GetBoardActionLogsQuery, Result<IEnumerable<GetBoardActionLogsResponse>>>
{
    public async Task<Result<IEnumerable<GetBoardActionLogsResponse>>> Handle(GetBoardActionLogsQuery request, CancellationToken cancellationToken)
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

        IEnumerable<BoardActionLog> logs = await logRepository.GetByBoardAsync(request.BoardId, cancellationToken);

        IEnumerable<GetBoardActionLogsResponse> response = logs.Select(log => new GetBoardActionLogsResponse(
            log.Id,
            log.UserId,
            log.Details,
            log.Timestamp
        ));

        logger.LogDebug("Fetched {Count} action logs for board {BoardId}", logs.Count(), request.BoardId);

        return Result.Success(response);
    }
}
