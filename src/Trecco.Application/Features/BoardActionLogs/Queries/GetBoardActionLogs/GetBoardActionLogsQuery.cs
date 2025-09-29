namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

public sealed record GetBoardActionLogsQuery(
    Guid BoardId,
    Guid UserId
) : IRequest<Result<IEnumerable<GetBoardActionLogsResponse>>>;
