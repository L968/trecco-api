using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Commands.AddMember;

internal sealed class AddMemberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:Guid}/members",
            async (
                Guid boardId,
                AddMemberRequest request,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var command = new AddMemberCommand(boardId, request.UserId, requesterId.Value);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Boards);
    }

    internal sealed record AddMemberRequest(Guid UserId);
}
