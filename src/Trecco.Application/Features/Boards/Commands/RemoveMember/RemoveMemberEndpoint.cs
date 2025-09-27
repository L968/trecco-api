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
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveMemberCommand(boardId, userId);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Boards);
    }
}
