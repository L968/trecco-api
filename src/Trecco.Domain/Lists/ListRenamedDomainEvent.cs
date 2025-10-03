using Trecco.Domain.DomainEvents;

namespace Trecco.Domain.Lists;

public sealed record ListRenamedDomainEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid ListId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public ListRenamedDomainEvent(Guid boardId, Guid listId, string oldName, string newName)
    {
        BoardId = boardId;
        ListId = listId;
        OldName = oldName;
        NewName = newName;
    }
}
