namespace Trecco.Application.Features.Boards.Queries.GetMyBoards;

public sealed record GetMyBoardsResponse(
    Guid Id,
    string Name
);
