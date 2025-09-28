using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Lists.CreateList;

internal sealed class CreateListEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:Guid}/list",
            async (
                Guid boardId,
                CreateListRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateListCommand(boardId, request.Name);
                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Lists);
    }

    internal sealed record CreateListRequest(string Name);
}
