using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trecco.Application.Domain.Users;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public User(string name)
    {
        Id = Guid.CreateVersion7();
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name cannot be empty.", nameof(name))
            : name;
    }
}
