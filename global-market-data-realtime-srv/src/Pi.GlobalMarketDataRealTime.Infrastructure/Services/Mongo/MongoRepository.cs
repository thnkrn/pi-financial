using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Pi.GlobalMarketDataRealTime.Infrastructure.Exceptions;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Mongo;

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
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to get entity with id {id}.", ex);
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

    public async Task CreateManyAsync(TEntity[] entities)
    {
        try
        {
            await _context.GetCollection<TEntity>().InsertManyAsync(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entities.");
            throw new MongoRepositoryException("Failed to create entities.", ex);
        }
    }

    public async Task UpdateAsync(string id, TEntity entity)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await _context.GetCollection<TEntity>().ReplaceOneAsync(filter, entity);
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
            var filter = Builders<TEntity>.Filter.Eq("_id", ObjectId.Parse(id));

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to upsert entity with id {id}.", ex);
        }
    }

    public async Task ReplaceAsync(ObjectId id, TEntity entity)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await _context.GetCollection<TEntity>().ReplaceOneAsync(filter, entity);
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
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await _context.GetCollection<TEntity>().DeleteOneAsync(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
    }

    public async Task DeleteByColumnAsync(string columnName, string columnValue)
    {
        try
        {
            var filter = Builders<TEntity>.Filter.Eq(columnName, columnValue);
            await _context.GetCollection<TEntity>().DeleteManyAsync(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with {ColumnName} {ColumnValue}.", columnName, columnValue);
            throw new MongoRepositoryException($"Failed to delete entity with {columnName} {columnValue}.", ex);
        }
    }

    public async Task DeleteByMultipleColumnAsync(Dictionary<string, object> columns)
    {
        try
        {
            var filterBuilder = Builders<TEntity>.Filter;
            FilterDefinition<TEntity>? filters = null;
            foreach (var (columnName, columnValue) in columns)
                if (filters == null)
                    filters = filterBuilder.Eq(columnName, columnValue);
                else
                    filters &= filterBuilder.Eq(columnName, columnValue);

            if (filters == null)
                throw new InvalidOperationException("Null Condition");

            await _context.GetCollection<TEntity>().DeleteManyAsync(filters);
        }
        catch (Exception ex)
        {
            var columnsVal = string.Join(" and ", columns.Select(e => e.Key + " = " + e.Value).ToArray());
            _logger.LogError(ex, "Failed to delete entity with {Columns}.", columnsVal);
            throw new MongoRepositoryException($"Failed to delete entity with {columnsVal}.", ex);
        }
    }

    public async Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .Find(filterExpression).ToListAsync();
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

    public async Task<long> CountAllAsync()
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .CountDocumentsAsync(_ => true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count all documents in collection.");
            throw new MongoRepositoryException("Failed to count all documents in collection.", ex);
        }
    }
}