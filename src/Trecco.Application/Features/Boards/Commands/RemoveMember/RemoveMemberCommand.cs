namespace Trecco.Application.Features.Boards.Commands.RemoveMember;

public sealed record RemoveMemberCommand(
    Guid BoardId,
    Guid UserId,
    Guid RequesterId
) : IRequest<Result>;
