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
            logger.LogWarning("Board {BoardId} not found when removing user {UserId}", request.BoardId, request.UserId);
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (!board.MemberIds.Contains(request.UserId))
        {
            logger.LogWarning("User {UserId} is not a member of Board {BoardId}", request.UserId, request.BoardId);
            return Result.Failure(BoardErrors.NotMember);
        }

        board.RemoveMember(request.UserId);
        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogInformation("User {UserId} removed from Board {BoardId}", request.UserId, request.BoardId);

        return Result.Success();
    }
}
