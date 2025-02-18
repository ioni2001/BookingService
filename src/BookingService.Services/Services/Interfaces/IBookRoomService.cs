using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;

namespace BookingService.Services.Services.Interfaces;

public interface IBookRoomService
{
    Task<Booking> CreateBookingAsync(CreateBookingRequest createBookingRequest);

    Task<List<RoomAvailabilityDto>> GetRoomAvailability(string roomId, DateOnly date);

    Task<List<Booking>> GetUserBookingsAsync(string userId);

    Task<List<Room>> GetAllRooms();
}
