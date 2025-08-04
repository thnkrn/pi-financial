using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Exceptions;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.TimescaleEf;

public class TimescaleRepository<TEntity> : ITimescaleRepository<TEntity> where TEntity : class
{
    private readonly TimescaleContext _context;
    private readonly ILogger<TimescaleRepository<TEntity>> _logger;

    public TimescaleRepository(TimescaleContext context, ILogger<TimescaleRepository<TEntity>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new TimescaleRepositoryException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get entity with id {id}.");
            throw new TimescaleRepositoryException($"Failed to get entity with id {id}.", ex);
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new TimescaleRepositoryException("Failed to create entity.", ex);
        }
    }

    public async Task UpdateAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new TimescaleRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete entity with id {id}.");
            throw new TimescaleRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
    }
}