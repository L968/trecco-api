namespace Trecco.Application.Features.Boards.Queries.GetBoardsByOwner;

public sealed record GetBoardsByOwnerResponse(
    Guid Id,
    string Name,
    Guid OwnerUserId
);
