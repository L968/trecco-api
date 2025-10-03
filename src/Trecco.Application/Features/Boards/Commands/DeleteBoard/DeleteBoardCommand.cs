namespace Trecco.Application.Features.Boards.Commands.DeleteBoard;

public sealed record DeleteBoardCommand(
    Guid BoardId,
    Guid RequesterId
) : IRequest<Result>;
