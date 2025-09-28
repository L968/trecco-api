using Microsoft.AspNetCore.Mvc;
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
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var command = new MoveCardCommand(
                    boardId,
                    cardId,
                    request.TargetListId,
                    request.TargetPosition,
                    requesterId.Value
                );

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }

    internal sealed record MoveCardRequest(
        Guid TargetListId,
        int TargetPosition
    );
}
