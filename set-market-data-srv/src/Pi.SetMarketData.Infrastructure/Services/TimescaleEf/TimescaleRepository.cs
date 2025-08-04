using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

// ReSharper disable CoVariantArrayConversion
public class TimescaleRepository<TEntity> : ITimescaleRepository<TEntity>
    where TEntity : class
{
    private readonly IDbContextFactory<TimescaleContext> _contextFactory;
    private readonly ILogger<TimescaleRepository<TEntity>> _logger;
    private readonly ConcurrentQueue<TimescaleContext> _contextPool;
    private readonly SemaphoreSlim _poolLock;
    private readonly int _maxPoolSize;
    private int _poolSize;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="contextFactory"></param>
    /// <param name="logger"></param>
    public TimescaleRepository(
        IConfiguration configuration,
        IDbContextFactory<TimescaleContext> contextFactory,
        ILogger<TimescaleRepository<TEntity>> logger
    )
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _maxPoolSize = configuration.GetValue(ConfigurationKeys.TimescalePoolSize, 20);
        _contextPool = new ConcurrentQueue<TimescaleContext>();
        _poolLock = new SemaphoreSlim(1, 1);
        _poolSize = 0;
        _disposed = false;
    }

    private async Task<TimescaleContext> GetContextAsync()
    {
        if (_contextPool.TryDequeue(out var context))
        {
            return context;
        }

        await _poolLock.WaitAsync();
        try
        {
            if (_contextPool.TryDequeue(out context))
            {
                return context;
            }

            if (_poolSize < _maxPoolSize)
            {
                context = await _contextFactory.CreateDbContextAsync();
                _poolSize++;
                return context;
            }
            else
            {
                // Wait for a context to be returned to the pool
                _logger.LogWarning("Connection pool limit reached. Waiting for a connection to become available.");
                // Wait for a connection to be released
                while (!_contextPool.TryDequeue(out context))
                {
                    await Task.Delay(50);
                }
                return context;
            }
        }
        finally
        {
            _poolLock.Release();
        }
    }

    private void ReturnContext(TimescaleContext context)
    {
        if (context != null && !_disposed)
        {
            _contextPool.Enqueue(context);
        }
        else
        {
            // If we're disposing, dispose the context
            context?.Dispose();
            if (_poolSize > 0)
            {
                _poolSize--;
            }
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            return await context.Set<TEntity>().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new TimescaleRepositoryException("Failed to get all entities.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }


    public async Task<TEntity?> GetByIdAsync(int id)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            return await context.Set<TEntity>().FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new TimescaleRepositoryException($"Failed to get entity with id {id}.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            return await context.Set<TEntity>()
                .Where(filterExpression)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with error {Error}", ex.Message);
            throw new TimescaleRepositoryException("Failed to get entity with error .", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<TResult?> GetSelectedHighestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    )
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var result =
                await context
                    .Set<TEntity>()
                    .Where(filterExpression)
                    .OrderByDescending(keySelector) // Sort in descending order to get the highest value
                    .Select(selector)
                    .FirstOrDefaultAsync() ?? default;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with error {Error}", ex.Message);
            throw new TimescaleRepositoryException("Failed to get entity with error .", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<TResult?> GetSelectedLowestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    )
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var result =
                await context
                    .Set<TEntity>()
                    .Where(filterExpression)
                    .OrderBy(keySelector) // Sort in ascending order to get the lowest value
                    .Select(selector)
                    .FirstOrDefaultAsync() ?? default;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with error {Error}", ex.Message);
            throw new TimescaleRepositoryException("Failed to get entity with error .", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            await context.Set<TEntity>().AddAsync(entity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new TimescaleRepositoryException("Failed to create entity.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task UpdateAsync(int id, TEntity entity)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var existingEntity = await context.Set<TEntity>().FindAsync(id);
            if (existingEntity == null)
                throw new TimescaleRepositoryException(
                    $"Entity with id {id} not found.",
                    new Exception()
                );

            context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict while updating entity.");
            throw new TimescaleRepositoryException(
                "Concurrency conflict while updating entity.",
                ex
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new TimescaleRepositoryException("Failed to update entity.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task DeleteAsync(int id)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                context.Set<TEntity>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new TimescaleRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task BatchUpsertAsync(
        IEnumerable<TEntity> entities,
        string tableName,
        string[] conflictKeys)
    {
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentNullException.ThrowIfNull(conflictKeys);

        if (conflictKeys.Length == 0)
            throw new ArgumentException("Conflict keys must be provided", nameof(conflictKeys));

        TimescaleContext? context = null;
        context = await GetContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidOperationException("Table name could not be determined.");

            var properties = typeof(TEntity).GetProperties();
            var columnMappings = GetColumnMappings(properties);

            var columns = string.Join(", ", columnMappings.Select(m => EscapeIdentifier(m.ColumnName)));
            var conflictKeyString = string.Join(", ",
                conflictKeys.Select(k => EscapeIdentifier(GetColumnName(columnMappings, k))));

            // Prepare the values and updates for the batch
            var valuesList = new List<string>();
            var parameters = new List<NpgsqlParameter>();

            foreach (var entity in entities)
            {
                var valueString = string.Join(", ", columnMappings.Select(m =>
                {
                    var paramName = $"@{m.PropertyName}{parameters.Count}";
                    parameters.Add(new NpgsqlParameter(paramName, GetPropertyValue(entity, m.PropertyName)));
                    return paramName;
                }));

                valuesList.Add($"({valueString})");
            }

            // Get non-conflict columns for update
            var updateColumns = columnMappings
                .Where(m => !conflictKeys.Contains(m.PropertyName))
                .Select(m => $"{EscapeIdentifier(m.ColumnName)} = EXCLUDED.{EscapeIdentifier(m.ColumnName)}");

            // Construct the full query for batch upsert
            var query = new StringBuilder()
                .Append($"INSERT INTO {EscapeIdentifier(tableName)} ({columns}) ")
                .Append($"VALUES {string.Join(", ", valuesList)} ")
                .Append($"ON CONFLICT ({conflictKeyString}) DO UPDATE SET ")
                .Append(string.Join(", ", updateColumns));

            await context.Database.ExecuteSqlRawAsync(query.ToString(), parameters.ToArray());
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Database update error during batch upsert operation.");
            throw new TimescaleRepositoryException("Database update error during batch upsert operation.", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to batch upsert entities.");
            throw new TimescaleRepositoryException("Failed to batch upsert entities.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    private object GetPropertyValue(TEntity entity, string propertyName)
    {
        var property = typeof(TEntity).GetProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException(
                $"Property '{propertyName}' not found on entity of type {typeof(TEntity).Name}");
        }

        var propertyValue = property.GetValue(entity);

        if (propertyValue == null)
        {
            throw new InvalidOperationException(
                $"PropertyValue is null '{propertyValue}' not found on entity of type {typeof(TEntity).Name}");
        }

        return propertyValue;
    }

    public async Task UpsertAsync(TEntity entity, string tableName, string[] conflictKeys)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(conflictKeys);

        if (conflictKeys.Length == 0)
            throw new ArgumentException("Conflict keys must be provided", nameof(conflictKeys));

        TimescaleContext? context = null;
        context = await GetContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (string.IsNullOrEmpty(tableName))
                throw new InvalidOperationException("Table name could not be determined.");

            var properties = typeof(TEntity).GetProperties();
            var columnMappings = GetColumnMappings(properties);
            var columns = string.Join(
                ", ",
                columnMappings.Select(m => EscapeIdentifier(m.ColumnName))
            );
            var values = string.Join(
                ", ",
                columnMappings.Select(m =>
                    new StringBuilder().Append('@').Append(m.PropertyName).ToString()
                )
            );
            var updates = string.Join(
                ", ",
                columnMappings
                    .Where(m => !conflictKeys.Contains(m.PropertyName))
                    .Select(m =>
                        new StringBuilder()
                            .Append(EscapeIdentifier(m.ColumnName))
                            .Append(" = EXCLUDED.")
                            .Append(EscapeIdentifier(m.ColumnName))
                            .ToString()
                    )
            );

            var conflictKeyString = string.Join(
                ", ",
                conflictKeys.Select(k => EscapeIdentifier(GetColumnName(columnMappings, k)))
            );

            const string insertTemplate = "INSERT INTO";
            var query = new StringBuilder()
                .Append($"{insertTemplate} ")
                .Append(EscapeIdentifier(tableName))
                .Append(" (")
                .Append(columns)
                .Append(") VALUES (")
                .Append(values)
                .Append(") ON CONFLICT (")
                .Append(conflictKeyString)
                .Append(") DO UPDATE SET ")
                .Append($"{updates};")
                .ToString();

            var parameters = GetNpgsqlParameters(entity, columnMappings);

            await context.Database.ExecuteSqlRawAsync(query, parameters); //NOSONAR
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(
                ex,
                "Database update error during upsert operation for entity type {EntityType}.",
                typeof(TEntity).Name
            );
            throw new TimescaleRepositoryException(
                new StringBuilder()
                    .Append("Database update error during upsert operation for entity type ")
                    .Append($"{typeof(TEntity).Name}.")
                    .ToString(),
                ex
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(
                ex,
                "Failed to upsert entity of type {EntityType}.",
                typeof(TEntity).Name
            );
            throw new TimescaleRepositoryException(
                $"Failed to upsert entity of type {typeof(TEntity).Name}.",
                ex
            );
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<List<CandleData>> GetCandlesAsync(
        string timeframe,
        string symbol,
        string venue,
        int limit,
        DateTime? startDate,
        DateTime? endDate
    )
    {
        TimescaleContext? context = null;
        try
        {
            if (typeof(TEntity) != typeof(CandleData))
                throw new InvalidOperationException(
                    "This method can only be used with CandleData entities."
                );

            context = await GetContextAsync();

            IQueryable<CandleData> query = GetCandleQuery(context, timeframe, symbol, venue, startDate, endDate);

            // Order by date time
            query = query.OrderByDescending(c => c.Date);
            // Limit item
            query = query.Take(limit);

            // Execute the query and return the results
            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Candle with Symbol {Symbol}.", symbol);
            throw new TimescaleRepositoryException(
                $"Failed to get Candle with Symbol {symbol}.",
                ex
            );
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    private IQueryable<CandleData> GetCandleQuery(TimescaleContext context, string timeframe, string symbol,
        string venue, DateTime? startDate, DateTime? endDate)
    {
        // Get the appropriate DbSet based on the timeframe

        IQueryable<CandleData> query = timeframe switch
        {
            CandleType.candle1Min => context.Candle1Min,
            CandleType.candle2Min => context.Candle2Min,
            CandleType.candle5Min => context.Candle5Min,
            CandleType.candle15Min => context.Candle15Min,
            CandleType.candle30Min => context.Candle30Min,
            CandleType.candle60Min => context.Candle1Hour,
            CandleType.candle120Min => context.Candle2Hour,
            CandleType.candle240Min => context.Candle4Hour,
            CandleType.candle1Day => context.Candle1Day,
            CandleType.candle1Week => context.Candle1Week,
            CandleType.candle1Month => context.Candle1Month,
            _ => throw new ArgumentException("Invalid timeframe", nameof(timeframe))
        };

        // Apply filters
        query = query.Where(c => c.Symbol == symbol && c.Venue == venue);

        if (startDate.HasValue)
        {
            query = query.Where(c => c.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.Date <= endDate.Value);
        }

        return query;
    }

    public async Task<List<TechnicalIndicators>> GetIndicatorsAsync(
        string timeframe,
        string symbol,
        string venue,
        DateTime startDate,
        DateTime endDate,
        int limit
    )
    {
        TimescaleContext? context = null;
        try
        {
            if (typeof(TEntity) != typeof(TechnicalIndicators))
                throw new InvalidOperationException(
                    "This method can only be used with TechnicalIndicators entities."
                );

            context = await GetContextAsync();
            // Get the appropriate DbSet based on the timeframe
            IQueryable<TechnicalIndicators> query = timeframe switch
            {
                CandleType.candle1Min => context.TechnicalIndicators1Min,
                CandleType.candle2Min => context.TechnicalIndicators2Min,
                CandleType.candle5Min => context.TechnicalIndicators5Min,
                CandleType.candle15Min => context.TechnicalIndicators15Min,
                CandleType.candle30Min => context.TechnicalIndicators30Min,
                CandleType.candle60Min => context.TechnicalIndicators1Hour,
                CandleType.candle120Min => context.TechnicalIndicators2Hour,
                CandleType.candle240Min => context.TechnicalIndicators4Hour,
                CandleType.candle1Day => context.TechnicalIndicators1Day,
                CandleType.candle1Week => context.TechnicalIndicators1Week,
                CandleType.candle1Month => context.TechnicalIndicators1Month,
                _ => throw new ArgumentException("Invalid timeframe", nameof(timeframe))
            };

            // Apply filters
            query = query.Where(c => c.Symbol == symbol && c.Venue == venue);

            // Apply date filtering
            query = query.Where(c => c.DateTime >= startDate && c.DateTime <= endDate);

            // Order by date time
            query = query.OrderByDescending(c => c.DateTime);

            // Limit item
            query = query.Take(limit);

            // Execute the query and return the results
            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Indicator with Symbol {Symbol}.", symbol);
            throw new TimescaleRepositoryException(
                $"Failed to get Indicator with Symbol {symbol}.",
                ex
            );
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task UpsertTechnicalIndicators(
        string timeframe,
        TechnicalIndicators technicalIndicators
    )
    {
        if (typeof(TEntity) != typeof(TechnicalIndicators))
        {
            throw new InvalidOperationException(
                "This method can only be used with TechnicalIndicators entities."
            );
        }

        try
        {
            var tableName = timeframe switch
            {
                CandleType.candle1Min => TechnicalIndicatorTable.TechnicalIndicators1Min,
                CandleType.candle2Min => TechnicalIndicatorTable.TechnicalIndicators2Min,
                CandleType.candle5Min => TechnicalIndicatorTable.TechnicalIndicators5Min,
                CandleType.candle15Min => TechnicalIndicatorTable.TechnicalIndicators15Min,
                CandleType.candle30Min => TechnicalIndicatorTable.TechnicalIndicators30Min,
                CandleType.candle60Min => TechnicalIndicatorTable.TechnicalIndicators1Hour,
                CandleType.candle120Min => TechnicalIndicatorTable.TechnicalIndicators2Hour,
                CandleType.candle240Min => TechnicalIndicatorTable.TechnicalIndicators4Hour,
                CandleType.candle1Day => TechnicalIndicatorTable.TechnicalIndicators1Day,
                CandleType.candle1Week => TechnicalIndicatorTable.TechnicalIndicators1Week,
                CandleType.candle1Month => TechnicalIndicatorTable.TechnicalIndicators1Month,
                _ => throw new ArgumentException("Invalid timeframe", nameof(timeframe))
            };

            await UpsertAsync(
                (TEntity)(object)technicalIndicators,
                tableName,
                [
                    nameof(TechnicalIndicators.DateTime),
                    nameof(TechnicalIndicators.Symbol),
                    nameof(TechnicalIndicators.Venue)
                ]
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upsert Technical Indicators with symbol {Symbol}.",
                technicalIndicators.Symbol
            );
            throw new TimescaleRepositoryException(
                $"Failed to upsert Technical Indicators with symbol {technicalIndicators.Symbol}.",
                ex
            );
        }
    }

    private static string EscapeIdentifier(string identifier)
    {
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    private static NpgsqlParameter[] GetNpgsqlParameters(
        TEntity entity,
        List<(string PropertyName, string ColumnName)> columnMappings
    )
    {
        return columnMappings
            .Select(m =>
            {
                var property = typeof(TEntity).GetProperty(m.PropertyName);
                return new NpgsqlParameter(
                    $"@{m.PropertyName}",
                    property?.GetValue(entity) ?? DBNull.Value
                );
            })
            .ToArray();
    }

    private static List<(string PropertyName, string ColumnName)> GetColumnMappings(
        IEnumerable<PropertyInfo> properties
    )
    {
        return properties
            .Select(p =>
            {
                var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                return (p.Name, columnAttr?.Name ?? p.Name);
            })
            .ToList();
    }

    private static string GetColumnName(
        List<(string PropertyName, string ColumnName)> mappings,
        string propertyName
    )
    {
        return mappings.Find(m => m.PropertyName == propertyName).ColumnName;
    }

    public async Task UpsertBatchTechnicalIndicators(
        string timeframe,
        IEnumerable<TechnicalIndicators> technicalIndicators
    )
    {
        if (typeof(TEntity) != typeof(TechnicalIndicators))
        {
            throw new InvalidOperationException(
                "This method can only be used with TechnicalIndicators entities."
            );
        }

        try
        {
            var tableName = timeframe switch
            {
                CandleType.candle1Min => TechnicalIndicatorTable.TechnicalIndicators1Min,
                CandleType.candle2Min => TechnicalIndicatorTable.TechnicalIndicators2Min,
                CandleType.candle5Min => TechnicalIndicatorTable.TechnicalIndicators5Min,
                CandleType.candle15Min => TechnicalIndicatorTable.TechnicalIndicators15Min,
                CandleType.candle30Min => TechnicalIndicatorTable.TechnicalIndicators30Min,
                CandleType.candle60Min => TechnicalIndicatorTable.TechnicalIndicators1Hour,
                CandleType.candle120Min => TechnicalIndicatorTable.TechnicalIndicators2Hour,
                CandleType.candle240Min => TechnicalIndicatorTable.TechnicalIndicators4Hour,
                CandleType.candle1Day => TechnicalIndicatorTable.TechnicalIndicators1Day,
                CandleType.candle1Week => TechnicalIndicatorTable.TechnicalIndicators1Week,
                CandleType.candle1Month => TechnicalIndicatorTable.TechnicalIndicators1Month,
                _ => throw new ArgumentException($"Invalid timeframe {timeframe}", nameof(timeframe))
            };

            var entities = technicalIndicators.Cast<TEntity>();
            await BatchUpsertAsync(
                entities,
                tableName,
                [
                    nameof(TechnicalIndicators.DateTime),
                    nameof(TechnicalIndicators.Symbol),
                    nameof(TechnicalIndicators.Venue)
                ]
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upsert Technical Indicators with symbol {Symbol}.",
                technicalIndicators.First().Symbol
            );
            throw new TimescaleRepositoryException(
                $"Failed to upsert Technical Indicators with symbol {technicalIndicators.First().Symbol}.",
                ex
            );
        }
    }

    public async Task<(double High, double Low)> GetHighestLowest52Weeks(string symbol, string venue, DateTime lookupDatetime)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var oneYearAgo = lookupDatetime.AddYears(-1);

            var query = new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("    MAX(highest_high) AS highest_high,")
                .AppendLine("    MIN(lowest_low) AS lowest_low")
                .AppendLine("FROM high_low_52week")
                .AppendLine("WHERE symbol = @symbol")
                .AppendLine("  AND venue = @venue")
                .AppendLine("  AND as_of_date BETWEEN @startDate AND @endDate");

            var parameters = new[]
            {
                new NpgsqlParameter("@symbol", symbol),
                new NpgsqlParameter("@venue", venue),
                new NpgsqlParameter("@startDate", oneYearAgo),
                new NpgsqlParameter("@endDate", lookupDatetime)
            };

            var paramLog = string.Join(", ", parameters.Select(p => $"{p.ParameterName}={p.Value}"));
            _logger.LogInformation("GetHighestLowest52Weeks query string: {Query}", query);
            _logger.LogInformation("GetHighestLowest52Weeks parameters: {ParamLog}", paramLog);

            var result = await context.Set<HighLowResult>()
                .FromSqlRaw(query.ToString(), parameters)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                _logger.LogError("Failed to get 52-week high/low for symbol {Symbol}. Result is empty", symbol);
            }

            return (result?.highest_high ?? 0, result?.lowest_low ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get 52-week high/low for symbol {Symbol}.", symbol);
            throw new TimescaleRepositoryException(
                $"Failed to get 52-week high/low for symbol {symbol}.",
                ex
            );
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<List<RankingItem>> GetRankingItem(string[] symbols, string venue, int limit, DateTime rankingStartDate)
    {
        if (string.IsNullOrEmpty(venue))
        {
            return [];
        }

        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();
            var query = new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("    MIN(bucket) AS bucket,")
                .AppendLine("    venue, symbol,")
                .AppendLine("    SUM(amount) AS amount")
                .AppendLine("FROM candle_1_min")
                .AppendLine("WHERE")
                .AppendLine("    venue = @venue");

            // Dynamically create the IN clause with individual parameters
            if (symbols.Length > 0)
            {
                query.Append("    AND symbol IN (");
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (i > 0) query.Append(", ");
                    query.Append($"@symbol{i}");
                }
                query.AppendLine(")");
            }
            else
            {
                // If no symbols are provided, use a condition that will return no results
                query.AppendLine("    AND 1=0");
            }

            query
                .AppendLine("    AND bucket >= @rankingStartDate")
                .AppendLine("    AND bucket <= NOW()")
                .AppendLine("GROUP BY venue, symbol")
                .AppendLine("ORDER BY amount DESC")
                .AppendLine("LIMIT @limit");

            // Create a list to hold parameters
            var parametersList = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@rankingStartDate", rankingStartDate),
                new NpgsqlParameter("@venue", venue),
                new NpgsqlParameter("@limit", limit)
            };

            // Add a parameter for each symbol
            for (int i = 0; i < symbols.Length; i++)
            {
                parametersList.Add(new NpgsqlParameter($"@symbol{i}", symbols[i]));
            }

            var parameters = parametersList.ToArray();

            var paramLog = string.Join(", ", parameters.Select(p => $"{p.ParameterName}={p.Value}"));
            _logger.LogInformation("GetRankingItem query string: {Query}", query);
            _logger.LogInformation("GetRankingItem parameters: {ParamLog}", paramLog);


            using var cmd = new NpgsqlCommand(query.ToString(), context.Database.GetDbConnection() as NpgsqlConnection);
            cmd.Parameters.AddRange(parameters);

            if (context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                await context.Database.GetDbConnection().OpenAsync();
            }

            var result = new List<RankingItem>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new RankingItem
                {
                    Date = reader.GetDateTime(0),
                    Venue = reader.GetString(1),
                    Symbol = reader.GetString(2),
                    AmountDouble = reader.GetDouble(3)
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Most Active Symbols for venue {Venue}.", venue);
            throw new TimescaleRepositoryException($"Failed to get Most Active Symbols for venue {venue}.", ex);
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }

    public async Task<DateTime> GetFirstCandle(string symbol, string venue)
    {
        TimescaleContext? context = null;
        try
        {
            context = await GetContextAsync();

            var query = new StringBuilder()
                .AppendLine("SELECT COALESCE(MIN(bucket),NOW()) as earliest_candle ")
                .AppendLine("FROM candle_1_day ")
                .AppendLine("WHERE symbol = @symbol ")
                .AppendLine("AND venue = @venue ")
                .AppendLine("AND bucket >= NOW() - INTERVAL '6 years'")
                .AppendLine("LIMIT 1").ToString();

            var parameters = new[]
            {
                new NpgsqlParameter("@symbol", symbol),
                new NpgsqlParameter("@venue", venue)
            };

            var result = await context.Set<CandleDateResult>()
                .FromSqlRaw(query.ToString(), parameters)
                .FirstOrDefaultAsync() ?? new CandleDateResult();

            return result?.earliest_candle ?? new DateTime();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get first candle for symbol {Symbol}.", symbol);
            throw new TimescaleRepositoryException(
                $"Failed to get earliest candle for symbol {symbol}.",
                ex
            );
        }
        finally
        {
            if (context != null)
            {
                ReturnContext(context);
            }
        }
    }
}
