using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models.Models.Entities;

public class User
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("email")]
    public required string Email { get; set; }

    [BsonElement("password")]
    public required string Password { get; set; }
}
