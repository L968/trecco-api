using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trecco.Application.Common.DomainEvents;
using Trecco.Application.Domain.Cards;
using Trecco.Application.Domain.Lists;

namespace Trecco.Application.Domain.Boards;

public sealed class Board : Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    [BsonRepresentation(BsonType.String)]
    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    [BsonElement("MemberIds")]
    [BsonRepresentation(BsonType.String)]
    private List<Guid> _memberIds = [];
    public IReadOnlyCollection<Guid> MemberIds => _memberIds.AsReadOnly();

    [BsonElement("Lists")]
    private List<List> _lists = [];
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

    public Card? GetCardById(Guid cardId)
    {
        return _lists
            .SelectMany(l => l.Cards)
            .FirstOrDefault(c => c.Id == cardId);
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
            AddDomainEvent(new MemberAddedDomainEvent(Id, userId));
        }
    }

    public void RemoveMember(Guid userId)
    {
        if (_memberIds.Remove(userId))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public Result CanRemoveMember(Guid requesterId, Guid targetUserId)
    {
        if (requesterId == OwnerUserId)
        {
            if (targetUserId == OwnerUserId)
            {
                return Result.Failure(BoardErrors.CannotRemoveOwner);
            }
        }
        else
        {
            if (requesterId != targetUserId)
            {
                return Result.Failure(BoardErrors.CannotRemoveOtherMember);
            }
        }

        return Result.Success();
    }

    public bool HasAccess(Guid userId)
    {
        return OwnerUserId == userId || _memberIds.Contains(userId);
    }

    public List AddList(string listName)
    {
        int position = _lists.Count > 0
            ? _lists.Max(l => l.Position) + 1
            : 0;

        var newList = new List(listName, position);
        _lists.Add(newList);

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ListAddedDomainEvent(Id, newList.Id, newList.Name));
        return newList;
    }

    public Result RenameList(Guid listId, string newName)
    {
        List? list = _lists.FirstOrDefault(l => l.Id == listId);
        if (list is null)
        {
            return Result.Failure(ListErrors.NotFound(listId));
        }

        string oldName = list.Name;
        list.UpdateName(newName);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ListRenamedDomainEvent(Id, list.Id, oldName, newName));

        return Result.Success();
    }

    public void RemoveList(Guid listId)
    {
        List? list = _lists.FirstOrDefault(l => l.Id == listId);

        if (list is not null)
        {
            _lists.Remove(list);
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ListDeletedDomainEvent(Id, list.Id, list.Name));
        }
    }

    public Result MoveCard(Guid cardId, Guid targetListId, int targetPosition)
    {
        List? sourceList = GetListByCardId(cardId);
        if (sourceList is null)
        {
            return Result.Failure(CardErrors.NotFound(cardId));
        }

        List? targetList = _lists.FirstOrDefault(l => l.Id == targetListId);
        if (targetList is null)
        {
            return Result.Failure(ListErrors.NotFound(targetListId));
        }

        Card card = sourceList.Cards.First(c => c.Id == cardId);

        sourceList.RemoveCard(cardId);
        targetList.InsertCard(card, targetPosition);

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CardMovedDomainEvent(Id, cardId, card.Title, targetListId, targetList.Name, targetPosition));

        return Result.Success();
    }

    public void DeleteCard(Guid cardId)
    {
        List? sourceList = GetListByCardId(cardId);
        if (sourceList is null)
        {
            return;
        }

        sourceList.RemoveCard(cardId);
        UpdatedAt = DateTime.UtcNow;
    }
}
