using Trecco.Domain.DomainEvents;

namespace Trecco.Domain.Cards;

public sealed record CardMovedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid CardId { get; }
    public string CardTitle { get; }
    public Guid TargetListId { get; }
    public string TargetListName { get; }
    public int TargetPosition { get; }

    public CardMovedDomainEvent(
        Guid boardId,
        Guid cardId,
        string cardTitle,
        Guid targetListId,
        string targetListName,
        int targetPosition
        )
    {
        BoardId = boardId;
        CardId = cardId;
        CardTitle = cardTitle;
        TargetListId = targetListId;
        TargetListName = targetListName;
        TargetPosition = targetPosition;
    }
}
