using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Commands.CreateBoard;

internal sealed class CreateBoardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("boards", async (CreateBoardCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            Result<CreateBoardResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(
                onSuccess: response => Results.Created($"/boards/{response.Id}", response),
                onFailure: ApiResults.Problem
            );
        })
        .WithTags(Tags.Boards);
    }
}
