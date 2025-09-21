namespace Trecco.Application.Features.Boards.Commands.CreateBoard;

public sealed record CreateBoardCommand(
    string Name
) : IRequest<Result<CreateBoardResponse>>;
