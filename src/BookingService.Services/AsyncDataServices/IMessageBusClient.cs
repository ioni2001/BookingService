using BookingService.Models.Models.Dtos;

namespace BookingService.Services.AsyncDataServices;

public interface IMessageBusClient
{
    void PublishBookingCreated(BookingCreatedMessage bookingCreatedMessage);
}
