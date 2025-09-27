using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Cards.MoveCard;

internal sealed class MoveCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards/{boardId:Guid}/cards/{cardId:Guid}/move",
            async (
                Guid boardId,
                Guid cardId,
                MoveCardRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new MoveCardCommand(
                    boardId,
                    cardId,
                    request.TargetListId,
                    request.TargetPosition
                );

                Result result = await sender.Send(command, cancellationToken);

                return result.Match( Results.NoContent,ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }
}

public sealed record MoveCardRequest(
    Guid TargetListId,
    int TargetPosition
);
