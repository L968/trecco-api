using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Queries.GetBoardById;

internal sealed class GetBoardByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("boards/{boardId:Guid}",
            async (
                Guid boardId,
                [FromHeader(Name = "X-User-Id")] Guid? userId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (userId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var query = new GetBoardByIdQuery(boardId, userId.Value);
                Result<GetBoardByIdResponse> result = await sender.Send(query, cancellationToken);

                return result.Match(
                    onSuccess: board => Results.Ok(board),
                    onFailure: ApiResults.Problem
                );
            })
        .WithTags(Tags.Boards);
    }
}
