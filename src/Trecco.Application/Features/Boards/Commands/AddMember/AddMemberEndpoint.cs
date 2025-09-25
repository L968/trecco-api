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
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new AddMemberCommand(boardId, request.UserId);

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent,ApiResults.Problem);
            })
        .WithTags(Tags.Boards);
    }
}

internal sealed record AddMemberRequest(Guid UserId);
