using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Cards.UpdateCard;

internal sealed class UpdateCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("boards/{boardId:Guid}/cards/{cardId:Guid}",
            async (
                Guid boardId,
                Guid listId,
                Guid cardId,
                UpdateCardRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateCardCommand(
                    boardId,
                    cardId,
                    request.Title,
                    request.Description
                );

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }

    public sealed record UpdateCardRequest(
        string Title,
        string Description
    );
}
