using BookingService.Models.Models.Dtos;
using BookingService.Services.Services.Interfaces;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BookingService.Services.Services.Implementations;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducerAccessor _producers;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IProducerAccessor producers, ILogger<KafkaProducerService> logger)
    {
        _producers = producers;
        _logger = logger;
    }

    public async Task ProduceBookingCreatedEventAsync(BookingCreatedMessage message)
    {
        var key = JsonConvert.SerializeObject(new JsonKey
        {
            RoomName = message.RoomName,
            UserName = message.UserName
        });

        var result = await _producers["booking-created-producer"].ProduceAsync(key, message);

        _logger.LogInformation("Booking Created in Room {RoomName} by {UserName} was successfully sent to Kafka" +
            " {KafkaContext.Topic} {KafkaContext.Partition} {KafkaContext.Offset}", message.RoomName, message.UserName, result.Topic, result.Partition, result.Offset);
    }

    private sealed class JsonKey
    {
        public required string RoomName { get; set; }
        public required string UserName { get; set; }
    }
}
