namespace Trecco.Application.Features.Boards.Queries.GetBoardsByOwner;

public sealed record GetBoardsByOwnerQuery(
    Guid OwnerUserId
) : IRequest<IEnumerable<GetBoardsByOwnerResponse>>;
