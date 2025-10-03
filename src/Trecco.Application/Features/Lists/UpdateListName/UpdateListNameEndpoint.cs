using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Lists.UpdateListName;

internal sealed class UpdateListNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("boards/{boardId:Guid}/lists/{listId:Guid}",
            async (
                Guid boardId,
                Guid listId,
                UpdateListNameRequest request,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var command = new UpdateListNameCommand(boardId, listId, request.Name, requesterId.Value);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Lists);
    }

    internal sealed record UpdateListNameRequest(string Name);
}
