using Trecco.Application.Common;

namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

public sealed record GetBoardActionLogsQuery(
    Guid BoardId,
    Guid UserId,
    int Page = 1,
    int PageSize = 15,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<GetBoardActionLogsResponse>>>;
