using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.SqlServer;

namespace Pi.SetMarketData.Infrastructure.Services.SqlServer;

public class SqlServerService<TEntity> : ISqlServerService<TEntity>
    where TEntity : class
{
    private readonly ILogger<SqlServerService<TEntity>> _logger;
    private readonly ISqlServerRepository<TEntity> _repository;

    /// <summary>
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public SqlServerService(
        ILogger<SqlServerService<TEntity>> logger,
        ISqlServerRepository<TEntity> repository
    )
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
            throw new SqlServerServiceException("Failed to get all entities.", ex);
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
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new SqlServerServiceException($"Failed to get entity with id {id}.", ex);
        }
    }
}