using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Exceptions;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.Infrastructure.Services.TimescaleEf;

public class TimescaleService<TEntity> : ITimescaleService<TEntity>
    where TEntity : class
{
    private readonly ILogger<TimescaleService<TEntity>> _logger;
    private readonly ITimescaleRepository<TEntity> _repository;

    public TimescaleService(
        ITimescaleRepository<TEntity> repository,
        ILogger<TimescaleService<TEntity>> logger
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
            throw new TimescaleServiceException("Failed to get all entities.", ex);
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
            throw new TimescaleServiceException($"Failed to get entity with id {id}.", ex);
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
            _logger.LogError(ex, "Failed to get entity with filter {Filter}.", filterExpression);
            throw new TimescaleServiceException(
                $"Failed to get entity with filter {filterExpression}.",
                ex
            );
        }
    }

    public async Task<IEnumerable<RealtimeMarketData?>> GetRealtimeMarketData(IEnumerable<GeInstrument> realtimeMarketData)
    {
        try
        {
            return await _repository.GetRealtimeMarketData(realtimeMarketData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get RealtimeMarketData");
            throw new TimescaleServiceException("Failed to get RealtimeMarketData", ex);
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

    public async Task UpdateAsync(int id, TEntity entity)
    {
        try
        {
            await _repository.UpdateAsync(id, entity);
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
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new TimescaleServiceException($"Failed to delete entity with id {id}.", ex);
        }
    }

    public async Task UpsertAsync(TEntity entity, params string[] conflictKeys)
    {
        try
        {
            await _repository.UpsertAsync(entity, conflictKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert entity.");
            throw new TimescaleServiceException("Failed to upsert entity.", ex);
        }
    }

    public async Task<List<CandleData>> GetCandlesAsync(
        string timeframe,
        string symbol,
        string venue,
        DateTime startDate,
        DateTime endDate,
        int limit
    )
    {
        try
        {
            return await _repository.GetCandlesAsync(
                timeframe,
                symbol,
                venue,
                startDate,
                endDate,
                limit
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Candle with Symbol {Symbol}.", symbol);
            throw new TimescaleServiceException($"Failed to get Candle with Symbol {symbol}.", ex);
        }
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
        try
        {
            return await _repository.GetIndicatorsAsync(
                timeframe,
                symbol,
                venue,
                startDate,
                endDate,
                limit
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Candle with Symbol {Symbol}.", symbol);
            throw new TimescaleServiceException($"Failed to get Candle with Symbol {symbol}.", ex);
        }
    }

    public async Task UpsertTechnicalIndicators(
        string timeframe,
        TechnicalIndicators technicalIndicators
    )
    {
        try
        {
            await _repository.UpsertTechnicalIndicators(timeframe, technicalIndicators);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upsert Technical Indicators with Symbol {Symbol}.",
                technicalIndicators.Symbol
            );
            throw new TimescaleServiceException(
                $"Failed to upsert Technical Indicators with Symbol {technicalIndicators.Symbol}.",
                ex
            );
        }
    }

    public async Task<TResult?> GetSelectedHighestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    )
    {
        try
        {
            return await _repository.GetSelectedHighestValueAsync(
                filterExpression,
                keySelector,
                selector
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with filter {Filter}.", filterExpression);
            throw new TimescaleServiceException(
                $"Failed to get entity with filter {filterExpression}.",
                ex
            );
        }
    }

    public async Task<TResult?> GetSelectedLowestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    )
    {
        try
        {
            return await _repository.GetSelectedLowestValueAsync(
                filterExpression,
                keySelector,
                selector
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with filter {Filter}.", filterExpression);
            throw new TimescaleServiceException(
                $"Failed to get entity with filter {filterExpression}.",
                ex
            );
        }
    }
    public async Task<(double High, double Low)> GetHighestLowest52Weeks(string symbol, string venue, DateTime lookupDatetime)
    {
        try
        {
            return await _repository.GetHighestLowest52Weeks(
                symbol,
                venue,
                lookupDatetime
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Highest Lowest Candles 52 Weeks with Symbol {Symbol}.", symbol);
            throw new TimescaleServiceException(
                $"Failed to get Highest Lowest Candles 52 Weeks with Symbol {symbol}.",
                ex
            );
        }
    }
    public async Task<DateTime> GetFirstCandle(string symbol, string venue)
    {
        try{
            return await _repository.GetFirstCandle(symbol, venue);
        }
        catch(Exception ex) {
            _logger.LogError(ex, "Faild to get first candle time");
            throw new TimescaleServiceException($"Failed to get First candle time for symbol {symbol}", ex);
        }
    }
    

    public async Task<List<RankingItem>> GetRankingItem(string[] symbols, string venue, int limit, DateTime rankingStartDate)
    {
        try
        {
            return await _repository.GetRankingItem(symbols, venue, limit, rankingStartDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Most Active Symbols with Symbols {Symbols}.", symbols);
            throw new TimescaleServiceException($"Failed to get Most Active Symbols with Symbols {symbols}.", ex);
        }
    }
}
