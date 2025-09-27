using MediatR;
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
                CreateCardRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCardCommand(
                    boardId,
                    listId,
                    request.Title,
                    request.Description
                );

                Result result = await sender.Send(command, cancellationToken);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
        .WithTags(Tags.Cards);
    }

    internal sealed record CreateCardRequest(string Title, string Description);
}
