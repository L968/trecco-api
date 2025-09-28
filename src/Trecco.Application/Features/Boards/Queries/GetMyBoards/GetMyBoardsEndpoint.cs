using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

internal sealed class GetMyBoardsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("boards/me",
            async (
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var query = new GetMyBoardsQuery(requesterId.Value);
                IEnumerable<GetMyBoardsResponse> boards = await sender.Send(query, cancellationToken);
                return Results.Ok(boards);
            })
        .WithTags(Tags.Boards);
    }
}
