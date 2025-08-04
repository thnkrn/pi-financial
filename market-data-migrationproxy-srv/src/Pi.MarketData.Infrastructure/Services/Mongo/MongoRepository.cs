using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Pi.MarketData.Infrastructure.Exceptions;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Services.Mongo;

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

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId)) return default;
            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to get entity with id {id}.", ex);
        }
    }

    public async Task<TEntity> GetBySymbolAsync(string symbol)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("symbol", symbol);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with symbol {Symbol}.", symbol);
            throw new MongoRepositoryException($"Failed to get entity with symbol {symbol}.", ex);
        }
    }

    public async Task<TEntity> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
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
                "Failed to get entities with current filter.{FilterExpression}",
                filterExpression
            );
            throw new MongoRepositoryException("Failed to get entities with current filter.", ex);
        }
    }

    public async Task<TEntity> GetMongoBySymbolAsync(string symbol)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("general_info.symbol", symbol);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with symbol {Symbol}.", symbol);
            throw new MongoRepositoryException($"Failed to get entity with symbol {symbol}.", ex);
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        try
        {
            await _context.GetCollection<TEntity>().InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new MongoRepositoryException("Failed to create entity.", ex);
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
                    typeof(TEntity).GetProperties()
                        .Where(property => property.GetValue(entity) != null)
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

    public async Task UpsertAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                var updateDefinitions = new List<UpdateDefinition<TEntity>>();
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    var propertyValue = property.GetValue(entity);
                    if (propertyValue == null)
                        continue;
                    if (
                        property.PropertyType.IsGenericType
                        && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                    )
                    {
                        // Property is a list, use PushEach to append elements
                        var pushUpdate = Builders<TEntity>.Update.PushEach(
                            property.Name,
                            (IEnumerable<object>)propertyValue
                        );
                        updateDefinitions.Add(pushUpdate);
                    }
                    else
                    {
                        // Property is not a list, use Set to replace the value
                        var setUpdate = Builders<TEntity>.Update.Set(property.Name, propertyValue);
                        updateDefinitions.Add(setUpdate);
                    }
                }

                var update = Builders<TEntity>.Update.Combine(updateDefinitions);
                var options = new UpdateOptions { IsUpsert = true };
                await _context.GetCollection<TEntity>().UpdateOneAsync(filter, update, options);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to upsert entity with id {id}.", ex);
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

    public async Task<TEntity?> GetByOrderBookId(string orderBookId)
    {
        try
        {
            if (!ObjectId.TryParse(orderBookId, out var objectId)) return default;
            var filter = Builders<TEntity>.Filter.Eq("order_book_id", objectId);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with orderBookId {OrderBookId}.", orderBookId);
            throw new MongoRepositoryException($"Failed to get entity with orderBookId {orderBookId}.", ex);
        }
    }
}