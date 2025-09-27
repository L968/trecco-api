using Trecco.Application.Common.DomainEvents;

namespace Trecco.Application.Domain.Cards;

public sealed record CardMovedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid CardId { get; }
    public Guid TargetListId { get; }
    public int TargetPosition { get; }

    public CardMovedDomainEvent(Guid boardId, Guid cardId, Guid targetListId, int targetPosition)
    {
        BoardId = boardId;
        CardId = cardId;
        TargetListId = targetListId;
        TargetPosition = targetPosition;
    }
}
