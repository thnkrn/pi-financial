using Microsoft.Extensions.Logging;
using Pi.SetMarketDataWSS.Infrastructure.Exceptions;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Timescale;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Timescale;

public class TimescaleService<TEntity> : ITimescaleService<TEntity> where TEntity : class
{
    private readonly ILogger<TimescaleService<TEntity>> _logger;
    private readonly ITimescaleRepository<TEntity> _repository;

    public TimescaleService(ITimescaleRepository<TEntity> repository, ILogger<TimescaleService<TEntity>> logger)
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
            throw new TimescaleServiceException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity> GetByIdAsync(int id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get entity with id {id}.");
            throw new TimescaleServiceException($"Failed to get entity with id {id}.", ex);
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
            throw new TimescaleServiceException("Failed to create entity.", ex);
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
            throw new TimescaleServiceException("Failed to update entity.", ex);
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
            throw new TimescaleServiceException($"Failed to delete entity with id {id}.", ex);
        }
    }
}