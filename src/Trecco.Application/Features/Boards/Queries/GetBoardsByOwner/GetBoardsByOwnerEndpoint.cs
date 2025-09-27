using Trecco.Application.Common.Endpoints;

namespace Trecco.Application.Features.Boards.Queries.GetBoardsByOwner;

internal sealed class GetBoardsByOwnerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("boards/owner/{ownerUserId:Guid}",
            async (
                Guid ownerUserId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetBoardsByOwnerQuery(ownerUserId);
                IEnumerable<GetBoardsByOwnerResponse> boards = await sender.Send(query, cancellationToken);
                return Results.Ok(boards);
            })
        .WithTags(Tags.Boards);
    }
}
