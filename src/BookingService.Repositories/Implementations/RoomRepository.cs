using BookingService.Models.Models.Configuration;
using BookingService.Models.Models.Entities;
using BookingService.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookingService.Repositories.Implementations;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    private readonly IMongoCollection<Room> _collection;
    private const string CollectionName = "Rooms";

    public RoomRepository(IOptions<MongoDbSettings> mongoSettings, ILogger<RoomRepository> logger)
        : base(InitializeCollection(mongoSettings), logger)
    {
        _collection = InitializeCollection(mongoSettings);
    }

    private static IMongoCollection<Room> InitializeCollection(IOptions<MongoDbSettings> mongoSettings)
    {
        var mongoDbSettings = mongoSettings.Value;
        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Uri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);
        var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        return database.GetCollection<Room>(CollectionName);
    }
}
