using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.SqlServer;

namespace Pi.SetMarketData.Infrastructure.Services.SqlServer;

public class SqlServerRepository<TEntity> : ISqlServerRepository<TEntity>
    where TEntity : class
{
    private readonly SqlServerContext _context;
    private readonly ILogger<SqlServerRepository<TEntity>> _logger;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public SqlServerRepository(
        SqlServerContext context,
        ILogger<SqlServerRepository<TEntity>> logger
    )
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
            throw new SqlServerRepositoryException("Failed to get all entities.", ex);
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
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new SqlServerRepositoryException($"Failed to get entity with id {id}.", ex);
        }
    }
}