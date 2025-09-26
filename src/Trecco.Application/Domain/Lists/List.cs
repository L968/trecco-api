using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trecco.Application.Domain.Cards;

namespace Trecco.Application.Domain.Lists;

public sealed class List
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Position { get; private set; }

    private readonly List<Card> _cards = [];
    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    public List(string name, int position)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Position = position;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        }

        Name = newName;
    }

    public Card AddCard(string title, string description)
    {
        int nextPosition = _cards.Count > 0
            ? _cards.Max(c => c.Position) + 1
            : 0;

        var card = new Card(title, description, nextPosition);
        _cards.Add(card);

        return card;
    }

    public void RemoveCard(Guid cardId)
    {
        Card? card = _cards.FirstOrDefault(c => c.Id == cardId);

        if (card is not null)
        {
            _cards.Remove(card);
        }
    }
}
