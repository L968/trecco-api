using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Commands.RemoveMember;

internal sealed class RemoveMemberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("boards/{boardId:Guid}/members/{userId:Guid}",
            async (
                Guid boardId,
                Guid userId,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var command = new RemoveMemberCommand(boardId, userId, requesterId.Value);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Boards);
    }
}
