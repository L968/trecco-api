using Trecco.Domain.DomainEvents;

namespace Trecco.Domain.Lists;

public sealed record ListDeletedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid ListId { get; }
    public string ListName { get; }

    public ListDeletedDomainEvent(Guid boardId, Guid listId, string listName)
    {
        BoardId = boardId;
        ListId = listId;
        ListName = listName;
    }
}
