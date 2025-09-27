using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Cards.DeleteCard;

internal sealed class DeleteCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("boards/{boardId:Guid}/cards/{cardId:Guid}",
            async (
                Guid boardId,
                Guid cardId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteCardCommand(boardId, cardId);
                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }
}
