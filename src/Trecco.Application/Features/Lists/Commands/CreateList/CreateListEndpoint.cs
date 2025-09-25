using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Lists.Commands.CreateList;

internal sealed class CreateListEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:guid}/lists",
            async (
                Guid boardId,
                CreateListCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                Result<CreateListResponse> result = await sender.Send(command with { BoardId = boardId }, ct);

                return result.Match(
                    onSuccess: response => Results.Created($"/boards/{boardId}/lists/{response.Id}", response),
                    onFailure: ApiResults.Problem
                );
            })
        .WithTags(Tags.Boards);
    }
}
