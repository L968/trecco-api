using Trecco.Domain.DomainEvents;

namespace Trecco.Domain.Boards;

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
