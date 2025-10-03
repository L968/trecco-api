using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Domain.Boards;

namespace Trecco.Application.Features.Boards.Commands.AddMember;

internal sealed class AddMemberHandler(
    IBoardRepository boardRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<AddMemberHandler> logger
) : IRequestHandler<AddMemberCommand, Result>
{
    public async Task<Result> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        Board? board = await boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            return Result.Failure(BoardErrors.NotFound(request.BoardId));
        }

        if (!board.HasAccess(request.RequesterId))
        {
            return Result.Failure(BoardErrors.NotAuthorized);
        }

        if (board.MemberIds.Contains(request.MemberId))
        {
            return Result.Failure(BoardErrors.AlreadyMember);
        }

        board.AddMember(request.MemberId);

        await boardRepository.UpdateAsync(board, cancellationToken);

        await domainEventDispatcher.DispatchAsync(board, cancellationToken);

        logger.LogDebug("User {UserId} added to Board {BoardId}", request.MemberId, request.BoardId);

        return Result.Success();
    }
}
