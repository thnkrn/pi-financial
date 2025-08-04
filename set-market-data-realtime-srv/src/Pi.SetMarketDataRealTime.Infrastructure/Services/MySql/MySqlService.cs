using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Exceptions;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.MySql;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.MySql;

public class MySqlService<TEntity> : IMySqlService<TEntity> where TEntity : class
{
    private readonly ILogger<MySqlService<TEntity>> _logger;
    private readonly IMySqlRepository<TEntity> _repository;

    public MySqlService(IMySqlRepository<TEntity> repository, ILogger<MySqlService<TEntity>> logger)
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
            throw new MySqlServiceException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get entity with id {id}.");
            throw new MySqlServiceException($"Failed to get entity with id {id}.", ex);
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
            throw new MySqlServiceException("Failed to create entity.", ex);
        }
    }

    public async Task UpdateAsync(TEntity entity)
    {
        try
        {
            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new MySqlServiceException("Failed to update entity.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete entity with id {id}.");
            throw new MySqlServiceException($"Failed to delete entity with id {id}.", ex);
        }
    }
}