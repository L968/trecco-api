using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trecco.Application.Domain.Boards;

public sealed class Board
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Board(string name)
    {
        Id = Guid.CreateVersion7();
        Name = name ?? throw new ArgumentNullException(nameof(name));
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
}
