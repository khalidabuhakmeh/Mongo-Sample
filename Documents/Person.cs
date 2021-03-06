using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Person
{
    internal const string Collection = "people";
    [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    // serialize dates into readable human format
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}