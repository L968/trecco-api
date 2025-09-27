using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Commands.RemoveMember;

internal sealed class RemoveMemberHandler(
    IBoardRepository boardRepository,
    ILogger<RemoveMemberHandler> logger
) : IRequestHandler<RemoveMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        board.RemoveMember(request.UserId);
        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogDebug("User {UserId} removed from Board {BoardId}", request.UserId, request.BoardId);

        return Result.Success();
    }
}
