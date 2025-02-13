using BookingService.Models.Models.Configuration;
using BookingService.Models.Models.Entities;
using BookingService.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections;

namespace BookingService.Repositories.Implementations;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly IMongoCollection<User> _collection;
    private const string CollectionName = "Users";

    public UserRepository(IOptions<MongoDbSettings> mongoSettings, ILogger<UserRepository> logger)
        : base(InitializeCollection(mongoSettings), logger)
    {
        _collection = InitializeCollection(mongoSettings);
    }


    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter
            .Eq(r => r.Email, email);

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    private static IMongoCollection<User> InitializeCollection(IOptions<MongoDbSettings> mongoSettings)
    {
        var mongoDbSettings = mongoSettings.Value;
        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Uri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var mongoClient = new MongoClient(settings);
        var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        return database.GetCollection<User>(CollectionName);
    }
}
