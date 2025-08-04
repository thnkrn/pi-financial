using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.SetMarketData.Domain.Models.Request.BrokerInfo;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Services.Mongo;

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

    public async Task<IEnumerable<TEntity>> GetAllByFiltersAsync(IEnumerable<Expression<Func<TEntity, bool>>> expressions)
    {
        try
        {
            return await _repository.GetAllByFiltersAsync(expressions);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with current filter.{@Expressions}",
                expressions
            );
            throw new MongoServiceException("Failed to get entity with current filter.", ex);
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

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector)
    {
        try
        {
            await _repository.UpdateManyAsync(entities, idSelector);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk upsert entities.");
            throw new MongoServiceException("Failed to bulk upsert entities.", ex);
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
    public async Task<BulkWriteResult?> UpsertManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector)
    {
        try
        {
            return await _repository.UpsertManyAsync(entities, idSelector);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert many entity");
            throw new MongoServiceException($"Failed to upsert many entity.", ex);
        }
    }

    public async Task UpsertAsyncByFilter(
        Expression<Func<TEntity, bool>> filterExpression,
        TEntity entity
    )
    {
        try
        {
            await _repository.UpsertAsyncByFilter(filterExpression, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, UpsertErrorMessage);
            throw new MongoServiceException(UpsertErrorMessage, ex);
        }
    }

    public async Task UpsertAsyncByFilterBsonDocument(
        Expression<Func<TEntity, bool>> filterExpression,
        TEntity entity
    )
    {
        try
        {
            await _repository.UpsertAsyncByFilterBsonDocument(filterExpression, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, UpsertErrorMessage);
            throw new MongoServiceException(UpsertErrorMessage, ex);
        }
    }

    public async Task UpsertBrokerInfoAsync(BrokerInfoRequest request)
    {
        await _repository.UpsertBrokerInfoAsync(request);
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
            _logger.LogError(
                ex,
                "Failed to get entity with orderBookId {OrderBookId}.",
                orderBookId
            );
            throw new MongoServiceException(
                $"Failed to get entity with orderBookId {orderBookId}.",
                ex
            );
        }
    }
}
