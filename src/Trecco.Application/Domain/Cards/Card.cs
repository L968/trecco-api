using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trecco.Application.Domain.Cards;

public sealed class Card
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Position { get; private set; }
    public DateTime CreatedBy { get; private set; }

    public Card(string title, string description, int position = 0)
    {
        Id = Guid.CreateVersion7();
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Title cannot be empty") : title;
        Description = description;
        Position = position;
        CreatedBy = DateTime.UtcNow;
    }

    public void SetPosition(int position)
    {
        Position = position;
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty");
        }

        Title = newTitle;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }
}
