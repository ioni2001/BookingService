using BookingService.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SharpCompress.Common;

namespace BookingService.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ILogger<Repository<T>> _logger;
    private readonly IMongoCollection<T> _collection;

    public Repository(
        IMongoCollection<T> collection,
        ILogger<Repository<T>> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task DeleteAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("id") ?? throw new InvalidOperationException("Entity must have an 'id' property.");
        var idValue = idProperty.GetValue(entity);

        var filter = Builders<T>.Filter
            .Eq("id", idValue);
        
        await _collection.DeleteOneAsync(filter);

        _logger.LogInformation("Entity of type {EntityType} with id {Id} was deleted successfully from collection {CollectionName}", entity.GetType().Name, idValue, _collection.CollectionNamespace.CollectionName);
    }

    public async Task<List<T>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter
            .Eq("_id", id);

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);

        _logger.LogInformation("Entity of type {EntityType} was saved successfully into collection {CollectionName}", entity.GetType().Name, _collection.CollectionNamespace.CollectionName);
    }

    public async Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("id") ?? throw new InvalidOperationException("Entity must have an 'id' property.");
        var idValue = idProperty.GetValue(entity);

        var filter = Builders<T>.Filter
            .Eq("id", idValue);

        var result = await _collection.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount > 0)
        {
            _logger.LogInformation("Entity of type {EntityType} with Id {Id} was updated in collection {CollectionName}", entity.GetType().Name, idValue, _collection.CollectionNamespace.CollectionName);
        }
        else
        {
            _logger.LogWarning("No entity of type {EntityType} with Id {Id} was found for updating in collection {CollectionName}", entity.GetType().Name, idValue, _collection.CollectionNamespace.CollectionName);
        }
    }
}
