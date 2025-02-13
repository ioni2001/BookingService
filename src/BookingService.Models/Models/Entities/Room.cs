using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models.Models.Entities;

public class Room
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("capacity")]
    public required int Capacity { get; set; }
}