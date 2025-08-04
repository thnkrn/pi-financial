using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pi.MarketData.Search.Infrastructure.Exceptions.Mongo;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;
namespace Pi.MarketData.Search.Infrastructure.Services.Mongo;

public class MongoRepository<TEntity> : IMongoRepository<TEntity>
    where TEntity : class
{
    private readonly IMongoContext _context;
    private readonly ILogger<MongoRepository<TEntity>> _logger;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public MongoRepository(IMongoContext context, ILogger<MongoRepository<TEntity>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _context.GetCollection<TEntity>().Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new MongoRepositoryException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with current filter.{FilterExpression}",
                filterExpression
            );
            throw new MongoRepositoryException("Failed to get entity with current filter.", ex);
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get all entity with current filter. {FilterExpression}",
                filterExpression
            );
            throw new MongoRepositoryException("Failed to get all entity with current filter.", ex);
        }
    }

    public async Task UpdateAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var bsonEntity = entity.ToBsonDocument();
                bsonEntity["_id"] = objectId; // Ensure the entity has the correct _id
                var updatedEntity = BsonSerializer.Deserialize<TEntity>(bsonEntity);
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                var updateDefinition = Builders<TEntity>.Update.Combine(
                    typeof(TEntity)
                        .GetProperties()
                        .Where(property => property.GetValue(entity) != null && property.Name != "IdString")
                        .Select(property =>
                        {
                            var value = property.GetValue(updatedEntity);
                            return Builders<TEntity>.Update.Set(property.Name, value);
                        })
                );

                await _context.GetCollection<TEntity>().UpdateOneAsync(filter, updateDefinition);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity with id {Id}.", id);
            throw new MongoRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task ReplaceAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                await _context.GetCollection<TEntity>().ReplaceOneAsync(filter, entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new MongoRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                await _context.GetCollection<TEntity>().DeleteOneAsync(filter);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
    }
}