using BookingService.Models.Models.Entities;

namespace BookingService.Repositories.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<List<Booking>> GetAllBookingsOfRoomInDate(string roomId, DateOnly bookingDate);

    Task<List<Booking>> GetBookingsOfRoomSortedByStartTime(string roomId, DateOnly bookingDate);

    Task<List<Booking>> GetUserBookingAsync(string userId);
}
