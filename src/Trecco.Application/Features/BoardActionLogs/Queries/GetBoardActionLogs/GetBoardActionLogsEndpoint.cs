using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.BoardActionLogs.Queries.GetBoardActionLogs;

internal sealed class GetBoardActionLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("boards/{boardId:Guid}/action-logs",
            async (
                Guid boardId,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 15
            ) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var query = new GetBoardActionLogsQuery(boardId, requesterId.Value, page, pageSize);
                Result<PaginatedList<GetBoardActionLogsResponse>> result = await sender.Send(query, cancellationToken);

                return result.Match(
                    onSuccess: logs => Results.Ok(logs),
                    onFailure: ApiResults.Problem
                );
            })
            .WithTags(Tags.Boards);

    }
}
