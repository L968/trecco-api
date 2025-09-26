using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trecco.Application.Domain.Cards;
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

    public List? GetListByCardId(Guid cardId)
    {
        return _lists.FirstOrDefault(l => l.Cards.Any(c => c.Id == cardId));
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
        if (!_memberIds.Contains(userId))
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
        int position = _lists.Count > 0
            ? _lists.Max(l => l.Position) + 1
            : 0;

        var newList = new List(listName, position);
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

    public void MoveCard(Guid cardId, Guid targetListId, int targetPosition)
    {
        List sourceList = GetListByCardId(cardId)
            ?? throw new InvalidOperationException("Card not found");

        Card card = sourceList.Cards.First(c => c.Id == cardId);

        List targetList = _lists.FirstOrDefault(l => l.Id == targetListId)
            ?? throw new InvalidOperationException("Target list not found");

        sourceList.RemoveCard(cardId);
        targetList.InsertCard(card, targetPosition);

        UpdatedAt = DateTime.UtcNow;
    }
}
