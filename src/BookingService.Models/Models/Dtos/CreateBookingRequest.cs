using MongoDB.Bson.Serialization.Attributes;

namespace BookingService.Models.Models.Dtos;

public class CreateBookingRequest
{
    public required string RoomId { get; set; }

    public required DateOnly BookingDate { get; set; }

    public required string StartTime { get; set; }

    public required string EndTime { get; set; }

    public required int NumberOfPersons { get; set; }

    public required string UserId { get; set; }
}
