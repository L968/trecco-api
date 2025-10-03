using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trecco.Domain.BoardActionLogs;

public sealed class BoardActionLog
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid BoardId { get; private set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; private set; }

    public string Details { get; private set; }

    public DateTime Timestamp { get; private set; }

    public BoardActionLog(Guid boardId, Guid userId, string details)
    {
        Id = Guid.CreateVersion7();
        BoardId = boardId;
        UserId = userId;
        Details = details ?? throw new ArgumentNullException(nameof(details));
        Timestamp = DateTime.UtcNow;
    }
}
