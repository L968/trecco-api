using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application.Domain.BoardActionLogs;

public sealed record BoardActionDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid UserId { get; }
    public string Details { get; }

    public BoardActionDomainEvent(Guid boardId, Guid userId, string details)
    {
        BoardId = boardId;
        UserId = userId;
        Details = details;
    }
}
