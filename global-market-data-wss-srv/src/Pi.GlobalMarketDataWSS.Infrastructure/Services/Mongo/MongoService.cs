using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Pi.GlobalMarketDataWSS.Infrastructure.Exceptions;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Mongo;

public class MongoService<TEntity> : IMongoService<TEntity>
    where TEntity : class
{
    private const string FilterFailed = "Failed to get entity with filterExpression";
    private readonly ILogger<MongoService<TEntity>> _logger;
    private readonly IMongoRepository<TEntity> _repository;

    /// <summary>
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public MongoService(IMongoRepository<TEntity> repository, ILogger<MongoService<TEntity>> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new MongoServiceException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new MongoServiceException($"Failed to get entity with id {id}.", ex);
        }
    }


    public async Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _repository.GetByFilterAsync(filterExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FilterFailed} {FilterExpression}.",
                FilterFailed, filterExpression
            );
            throw new MongoServiceException(
                $"{FilterFailed} {filterExpression}.",
                ex
            );
        }
    }

    public async Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _repository.GetListByFilterAsync(filterExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FilterFailed} {FilterExpression}.",
                FilterFailed, filterExpression
            );
            throw new MongoServiceException(
                $"{FilterFailed} {filterExpression}.",
                ex
            );
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        try
        {
            await _repository.CreateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new MongoServiceException("Failed to create entity.", ex);
        }
    }

    public async Task UpdateAsync(string id, TEntity entity)
    {
        try
        {
            await _repository.UpdateAsync(id, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new MongoServiceException("Failed to update entity.", ex);
        }
    }

    public async Task UpsertAsync(string id, TEntity entity)
    {
        try
        {
            await _repository.UpsertAsync(id, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new MongoServiceException("Failed to get entity with id {Id}.", ex);
        }
    }

    public async Task ReplaceAsync(ObjectId id, TEntity entity)
    {
        try
        {
            await _repository.ReplaceAsync(id, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert entity with id {Id}.", id);
            throw new MongoServiceException($"Failed to upsert entity with id {id}.", ex);
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new MongoServiceException($"Failed to delete entity with id {id}.", ex);
        }
    }
}