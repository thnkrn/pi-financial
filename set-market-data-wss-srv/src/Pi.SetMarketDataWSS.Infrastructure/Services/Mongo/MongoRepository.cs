using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.SetMarketDataWSS.Infrastructure.Exceptions;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Mongo;

public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class
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

    public async Task UpdateAsync(string id, TEntity entity)
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
}