using Trecco.Domain.DomainEvents;

namespace Trecco.Domain.Lists;

public sealed record ListAddedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid ListId { get; }
    public string ListName { get; }

    public ListAddedDomainEvent(Guid boardId, Guid listId, string listName)
    {
        BoardId = boardId;
        ListId = listId;
        ListName = listName;
    }
}
