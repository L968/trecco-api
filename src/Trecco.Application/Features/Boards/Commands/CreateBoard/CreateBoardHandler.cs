using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Commands.CreateBoard;

internal sealed class CreateBoardHandler(
    IBoardRepository boardRepository,
    ILogger<CreateBoardHandler> logger
    ) : IRequestHandler<CreateBoardCommand, Result<CreateBoardResponse>>
{
    public async Task<Result<CreateBoardResponse>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var board = new Board(request.Name, request.OwnerUserId);

        await boardRepository.InsertAsync(board, cancellationToken);

        logger.LogDebug("Successfully created {@Board}", board);

        return new CreateBoardResponse(
            board.Id,
            board.Name,
            board.OwnerUserId
        );
    }
}
