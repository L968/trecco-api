using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Commands.AddMember;

internal sealed class AddMemberHandler(
    IBoardRepository boardRepository,
    ILogger<AddMemberHandler> logger
) : IRequestHandler<AddMemberCommand, Result>
{
    public async Task<Result> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);

        if (board is null)
        {
            logger.LogWarning("Board {BoardId} not found when adding user {UserId}", request.BoardId, request.UserId);
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (board.MemberIds.Contains(request.UserId))
        {
            logger.LogInformation("User {UserId} is already a member of Board {BoardId}", request.UserId, request.BoardId);
            return Result.Failure(BoardErrors.AlreadyMember);
        }

        board.AddMember(request.UserId);

        await boardRepository.UpdateAsync(board, cancellationToken);

        logger.LogInformation("User {UserId} added to Board {BoardId}", request.UserId, request.BoardId);

        return Result.Success();
    }
}
