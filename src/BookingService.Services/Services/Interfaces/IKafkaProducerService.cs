using BookingService.Models.Models.Dtos;

namespace BookingService.Services.Services.Interfaces;

public interface IKafkaProducerService
{
    Task ProduceBookingCreatedEventAsync(BookingCreatedMessage message);
}
