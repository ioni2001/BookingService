using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models.Models.Entities;

public class Booking
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("roomId")]
    public required string RoomId { get; set; }

    [BsonElement("bookingDate")]
    public required DateOnly BookingDate { get; set; }

    [BsonElement("startTime")]
    public required TimeSpan StartTime { get; set; }

    [BsonElement("endTime")]
    public required TimeSpan EndTime { get; set; }

    [BsonElement("numberOfPersons")]
    public required int NumberOfPersons { get; set; }

    [BsonElement("userId")]
    public required string UserId { get; set; }
}
