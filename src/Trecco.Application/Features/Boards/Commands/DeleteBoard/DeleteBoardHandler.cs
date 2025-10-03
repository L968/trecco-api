using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardHandler(
    IBoardRepository boardRepository,
    ILogger<DeleteBoardHandler> logger
) : IRequestHandler<DeleteBoardCommand, Result>
{
    public async Task<Result> Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (request.RequesterId != board.OwnerUserId)
        {
            return Result.Failure(BoardErrors.NoPermission);
        }

        await boardRepository.DeleteAsync(board.Id, cancellationToken);

        logger.LogInformation("Board {BoardId} deleted by user {UserId}", board.Id, request.RequesterId);

        return Result.Success();
    }
}
