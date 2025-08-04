using Dapper;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Exceptions;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.MySql;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.MySql;

public class MySqlRepository<TEntity> : IMySqlRepository<TEntity> where TEntity : class
{
    private readonly MySqlContext _context;
    private readonly ILogger<MySqlRepository<TEntity>> _logger;
    private readonly string _tableName;

    public MySqlRepository(MySqlContext context, ILogger<MySqlRepository<TEntity>> logger)
    {
        _context = context;
        _tableName = typeof(TEntity).Name;
        _logger = logger;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            using var connection = _context.GetConnection();
            var query = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<TEntity>(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new MySqlRepositoryException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _context.GetConnection();
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get entity with id {id}.");
            throw new MySqlRepositoryException($"Failed to get entity with id {id}.", ex);
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        try
        {
            using var connection = _context.GetConnection();
            var columns = string.Join(", ", entity.GetType().GetProperties().Select(p => p.Name));
            var values = string.Join(", ", entity.GetType().GetProperties().Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
            await connection.ExecuteAsync(query, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new MySqlRepositoryException("Failed to create entity.", ex);
        }
    }

    public async Task UpdateAsync(TEntity entity)
    {
        try
        {
            using var connection = _context.GetConnection();
            var setClause = string.Join(", ", entity.GetType().GetProperties().Select(p => $"{p.Name} = @{p.Name}"));
            var query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            await connection.ExecuteAsync(query, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new MySqlRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            using var connection = _context.GetConnection();
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete entity with id {id}.");
            throw new MySqlRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
    }
}