using System.Linq.Expressions;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

public interface ITimescaleService<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<RealtimeMarketData?>> GetRealtimeMarketData(IEnumerable<GeInstrument> realtimeMarketData);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(int id, TEntity entity);
    Task DeleteAsync(int id);
    Task UpsertAsync(TEntity entity, params string[] conflictKeys);
    Task<List<CandleData>> GetCandlesAsync(
        string timeframe,
        string symbol,
        string venue,
        DateTime startDate,
        DateTime endDate,
        int limit
    );
    Task<List<TechnicalIndicators>> GetIndicatorsAsync(
        string timeframe,
        string symbol,
        string venue,
        DateTime startDate,
        DateTime endDate,
        int limit
    );
    Task UpsertTechnicalIndicators(string timeframe, TechnicalIndicators technicalIndicators);
    Task<TResult?> GetSelectedHighestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    );
    Task<TResult?> GetSelectedLowestValueAsync<TResult>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TResult>> keySelector,
        Expression<Func<TEntity, TResult>> selector
    );
    Task<(double High, double Low)> GetHighestLowest52Weeks(string symbol, string venue, DateTime lookupDatetime);
    Task<DateTime> GetFirstCandle(string symbol, string venue);
    Task<List<RankingItem>> GetRankingItem(string[] symbols, string venue, int limit, DateTime rankingStartDate);
}
