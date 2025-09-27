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
                var query = new GetBoardByIdQuery(boardId, userId);
                Result<GetBoardByIdResponse> result = await sender.Send(query, cancellationToken);

                return result.Match(
                    onSuccess: board => Results.Ok(board),
                    onFailure: ApiResults.Problem
                );
            })
        .WithTags(Tags.Boards);
    }
}
