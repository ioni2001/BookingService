using BookingService.Models.Models.Configuration;
using BookingService.Models.Models.Entities;
using BookingService.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookingService.Repositories.Implementations;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    private readonly IMongoCollection<Booking> _collection;
    private const string CollectionName = "Bookings";

    public BookingRepository(IOptions<MongoDbSettings> mongoSettings, ILogger<BookingRepository> logger)
        : base(InitializeCollection(mongoSettings), logger)
    {
        _collection = InitializeCollection(mongoSettings);
    }

    private static IMongoCollection<Booking> InitializeCollection(IOptions<MongoDbSettings> mongoSettings)
    {
        var mongoDbSettings = mongoSettings.Value;
        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Uri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);
        var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        return database.GetCollection<Booking>(CollectionName);
    }

    public async Task<List<Booking>> GetAllBookingsOfRoomInDate(string roomId, DateOnly bookingDate)
    {
        return await _collection
                .Find(b => b.RoomId == roomId && b.BookingDate == bookingDate)
                .ToListAsync();
    }

    public async Task<List<Booking>> GetBookingsOfRoomSortedByStartTime(string roomId, DateOnly bookingDate)
    {
        return await  _collection
        .Find(b => b.RoomId == roomId && b.BookingDate == bookingDate)
        .SortBy(b => b.StartTime)
        .ToListAsync();
    }

    public async Task<List<Booking>> GetUserBookingAsync(string userId)
    {
        return await _collection
        .Find(b => b.UserId == userId)
        .ToListAsync();
    }
}