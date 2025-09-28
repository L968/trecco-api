using Microsoft.AspNetCore.Mvc;
using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Cards.UpdateCard;

internal sealed class UpdateCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("boards/{boardId:Guid}/cards/{cardId:Guid}",
            async (
                Guid boardId,
                Guid cardId,
                UpdateCardRequest request,
                [FromHeader(Name = "X-User-Id")] Guid? requesterId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                if (requesterId is null)
                {
                    return Results.StatusCode(StatusCodes.Status403Forbidden);
                }

                var command = new UpdateCardCommand(
                    boardId,
                    cardId,
                    request.Title,
                    request.Description,
                    requesterId.Value
                );

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }

    internal sealed record UpdateCardRequest(
        string Title,
        string Description
    );
}
