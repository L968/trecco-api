using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application.Domain.Boards;

public sealed record MemberAddedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid UserId { get; }

    public MemberAddedDomainEvent(Guid boardId, Guid userId)
    {
        BoardId = boardId;
        UserId = userId;
    }
}
