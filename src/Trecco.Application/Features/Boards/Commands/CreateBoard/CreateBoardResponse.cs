namespace Trecco.Application.Features.Boards.Commands.CreateBoard;

public sealed record CreateBoardResponse(
    Guid Id,
    string Name
);
