using AutoMapper;
using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using BookingService.Repositories.Interfaces;
using BookingService.Services.AsyncDataServices;
using BookingService.Services.Exceptions;
using BookingService.Services.Services.Interfaces;
using System.Globalization;

namespace BookingService.Services.Services.Implementations;

public class BookRoomService : IBookRoomService
{
    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IKafkaProducerService _kafkaProducerService;


    public BookRoomService(
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IMapper mapper,
        IMessageBusClient messageBusClient,
        IKafkaProducerService kafkaProducerService)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _messageBusClient = messageBusClient;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<Booking> CreateBookingAsync(CreateBookingRequest createBookingRequest)
    {
        // Validate room exists
        var room = await _roomRepository.GetByIdAsync(createBookingRequest.RoomId) ?? throw new NotFoundException($"Room with id {createBookingRequest.RoomId} was not found.");

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(createBookingRequest.UserId) ?? throw new NotFoundException($"User with id {createBookingRequest.UserId} was not found.");

        // Validate room capacity
        if (createBookingRequest.NumberOfPersons > room.Capacity)
        {
            throw new BadRequestException("Number of persons exceeds room capacity.");
        }

        TimeSpan startTime = TimeSpan.ParseExact(createBookingRequest.StartTime, @"hh\:mm", CultureInfo.InvariantCulture);
        TimeSpan endTime = TimeSpan.ParseExact(createBookingRequest.EndTime, @"hh\:mm", CultureInfo.InvariantCulture);

        // Validate time range
        if (startTime >= endTime)
        {
            throw new BadRequestException("Invalid time range: Start time must be earlier than end time.");
        }

        // Validate room availability
        bool isAvailable = await IsRoomAvailableAsync(createBookingRequest);

        if (!isAvailable)
        {
            throw new BadRequestException("Room is not available for the requested date and time range.");
        }

        // Save booking
        var booking = _mapper.Map<Booking>(createBookingRequest);

        await _bookingRepository.SaveAsync(booking);

        // Send RabbitMQ Event
        var bookingCreatedMessage = _mapper.Map<BookingCreatedMessage>(createBookingRequest);
        bookingCreatedMessage.UserName = user.Name;
        bookingCreatedMessage.RoomName = room.Name;

        _messageBusClient.PublishBookingCreated(bookingCreatedMessage);

        // Send Kafka Event
        bookingCreatedMessage.Event = user.Email;
        await _kafkaProducerService.ProduceBookingCreatedEventAsync(bookingCreatedMessage);


        return booking;
    }

    public async Task<List<Room>> GetAllRooms() => await _roomRepository.GetAllAsync();

    public async Task<List<RoomAvailabilityDto>> GetRoomAvailability(string roomId, DateOnly date)
    {
        // Operating hours
        var openingTime = TimeSpan.FromHours(8); // 08:00
        var closingTime = TimeSpan.FromHours(18); // 18:00

        var bookingsSortedByStartTime = await _bookingRepository.GetBookingsOfRoomSortedByStartTime(roomId, date);

        var availableIntervals = new List<(TimeSpan Start, TimeSpan End)>();

        var lastEndTime = openingTime;
        foreach (var booking in bookingsSortedByStartTime)
        {
            if (booking.StartTime > lastEndTime)
            {
                // There's a gap between the previous booking and this one
                availableIntervals.Add((lastEndTime, booking.StartTime));
            }

            lastEndTime = booking.EndTime > lastEndTime ? booking.EndTime : lastEndTime;
        }

        // Add final interval if there's time after the last booking
        if (lastEndTime < closingTime)
        {
            availableIntervals.Add((lastEndTime, closingTime));
        }

        return availableIntervals.Select(interval => new RoomAvailabilityDto
        {
            Start = interval.Start.ToString(@"hh\:mm"),
            End = interval.End.ToString(@"hh\:mm")
        }).ToList();
    }

    public async Task<List<Booking>> GetUserBookingsAsync(string userId) => await _bookingRepository.GetUserBookingAsync(userId);

    private async Task<bool> IsRoomAvailableAsync(CreateBookingRequest createBookingRequest)
    {
        var bookings = await _bookingRepository.GetAllBookingsOfRoomInDate(createBookingRequest.RoomId, createBookingRequest.BookingDate);

        var openingTime = TimeSpan.FromHours(8); // 08:00
        var closingTime = TimeSpan.FromHours(18); // 18:00

        TimeSpan startTime = TimeSpan.ParseExact(createBookingRequest.StartTime, @"hh\:mm", CultureInfo.InvariantCulture);
        TimeSpan endTime = TimeSpan.ParseExact(createBookingRequest.EndTime, @"hh\:mm", CultureInfo.InvariantCulture);

        if (startTime < openingTime || endTime > closingTime)
        {
            return false;
        }

        // Check for overlap
        foreach (var booking in bookings)
        {
            if (startTime < booking.EndTime && endTime > booking.StartTime)
            {
                return false; // Overlapping booking
            }
        }

        return true;
    }

}
