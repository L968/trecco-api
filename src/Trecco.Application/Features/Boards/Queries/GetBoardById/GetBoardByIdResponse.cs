namespace Trecco.Application.Features.Boards.Queries.GetBoardById;

public sealed record GetBoardByIdResponse(
    Guid Id,
    string Name,
    Guid OwnerUserId,
    DateTime LastUpdate,
    IReadOnlyCollection<Guid> MemberIds,
    IReadOnlyCollection<BoardListDto> Lists
);

public sealed record BoardListDto(
    Guid Id,
    string Name,
    int Position,
    IReadOnlyCollection<BoardCardDto> Cards
);

public sealed record BoardCardDto(
    Guid Id,
    string Title,
    string Description,
    int Position
);
