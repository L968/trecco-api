using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Domain.Boards;

public sealed class Board
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Guid> _memberIds = [];
    public IReadOnlyCollection<Guid> MemberIds => _memberIds.AsReadOnly();

    private readonly List<List> _lists = [];
    public IReadOnlyCollection<List> Lists => _lists.AsReadOnly();

    public Board(string name, Guid ownerUserId)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        OwnerUserId = ownerUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        }

        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMember(Guid userId)
    {
        if (!MemberIds.Contains(userId))
        {
            _memberIds.Add(userId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveMember(Guid userId)
    {
        if (_memberIds.Remove(userId))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public List AddList(string listName)
    {
        var newList = new List(listName);
        _lists.Add(newList);
        UpdatedAt = DateTime.UtcNow;
        return newList;
    }

    public void RemoveList(Guid listId)
    {
        List? list = _lists.FirstOrDefault(l => l.Id == listId);

        if (list is not null)
        {
            _lists.Remove(list);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
