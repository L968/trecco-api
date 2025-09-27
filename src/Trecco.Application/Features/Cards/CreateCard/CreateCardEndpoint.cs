using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Cards.CreateCard;

internal sealed class CreateCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:Guid}/lists/{listId:Guid}/cards",
            async (
                Guid boardId,
                Guid listId,
                CreateCardCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                Result<CreateCardResponse> result = await sender.Send(command with { BoardId = boardId, ListId = listId }, ct);

                return result.Match(
                    onSuccess: response => Results.Created($"/boards/{boardId}/lists/{listId}/cards/{response.Id}", response),
                    onFailure: ApiResults.Problem
                );
            })
        .WithTags(Tags.Cards);
    }
}
