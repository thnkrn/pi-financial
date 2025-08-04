using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Pi.MarketData.Search.Infrastructure.Exceptions.Mongo;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Search.Infrastructure.Services.Mongo;

public class MongoService<TEntity> : IMongoService<TEntity>
    where TEntity : class
{
    private const string UpsertErrorMessage = "Failed to upsert entity with filterExpression.";
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

    public async Task<TEntity?> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _repository.GetOneByFilterAsync(filterExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with current filter.{FilterExpression}",
                filterExpression
            );
            throw new MongoServiceException("Failed to get entity with current filter.", ex);
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _repository.GetAllByFilterAsync(filterExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with current filter.{FilterExpression}",
                filterExpression
            );
            throw new MongoServiceException("Failed to get entity with current filter.", ex);
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

    public async Task ReplaceAsync(string id, TEntity entity)
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