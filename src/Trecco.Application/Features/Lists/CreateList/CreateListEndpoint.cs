using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Lists.CreateList;

internal sealed class CreateListEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:Guid}/lists",
            async (
                Guid boardId,
                CreateListRequest request,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.Forbid();
                }

                var command = new CreateListCommand(boardId, request.Name, requesterId.Value);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Lists);
    }

    internal sealed record CreateListRequest(string Name);
}
