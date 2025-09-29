namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

public sealed record GetBoardActionLogsResponse(
    Guid Id,
    Guid UserId,
    string Details,
    DateTime Timestamp
);
