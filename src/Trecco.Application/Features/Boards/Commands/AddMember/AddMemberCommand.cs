namespace Trecco.Application.Features.Boards.Commands.AddMember;

public sealed record AddMemberCommand(
    Guid BoardId,
    Guid MemberId,
    Guid RequesterId
) : IRequest<Result>;
