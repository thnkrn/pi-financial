using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Pi.MarketData.Infrastructure.Exceptions;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Services.Mongo;

public class MongoService<TEntity> : IMongoService<TEntity>
    where TEntity : class
{
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

    public async Task<TEntity> GetBySymbolAsync(string symbol)
    {
        try
        {
            return await _repository.GetBySymbolAsync(symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with symbol {Symbol}.", symbol);
            throw new MongoServiceException($"Failed to get entity with symbol {symbol}.", ex);
        }
    }

    public async Task<TEntity> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _repository.GetByFilterAsync(filterExpression);
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

    public async Task<TEntity> GetMongoBySymbolAsync(string symbol)
    {
        try
        {
            return await _repository.GetMongoBySymbolAsync(symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with symbol {Symbol}.", symbol);
            throw new MongoServiceException($"Failed to get entity with symbol {symbol}.", ex);
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
            _logger.LogError(ex, "Failed to upsert entity with id {Id}.", id);
            throw new MongoServiceException($"Failed to upsert entity with id {id}.", ex);
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

    public async Task<TEntity?> GetByOrderBookId(string orderBookId)
    {
        try
        {
            return await _repository.GetByOrderBookId(orderBookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with orderBookId {OrderBookId}.", orderBookId);
            throw new MongoServiceException($"Failed to get entity with orderBookId {orderBookId}.", ex);
        }
    }
}