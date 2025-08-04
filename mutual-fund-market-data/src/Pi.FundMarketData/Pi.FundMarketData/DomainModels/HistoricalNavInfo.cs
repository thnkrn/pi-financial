using MongoDB.Driver.Linq;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class HistoricalNavInfo
{
    public string Symbol { get; init; }
    public decimal? NavChange { get; init; }
    public double? NavChangePercentage { get; init; }
    public IList<HistoricalNav> NavList { get; init; } = new List<HistoricalNav>();
    public HistoricalNavInfo(IList<HistoricalNav> entireNavs, Interval interval)
    {
        if (entireNavs == null)
            throw new ArgumentNullException(nameof(entireNavs), "navList is required.");

        if (entireNavs.Count == 0)
            throw new ArgumentException("navList cannot be empty.", nameof(entireNavs));

        var orderedDescNavs = entireNavs.OrderByDescending(q => q.Date).ToArray();

        var latest = orderedDescNavs[0];
        var (start, end) = interval.GetIntervalDateTimes(latest.Date);
        var boundedNavsDesc = orderedDescNavs.Where(x => x.Date >= start && x.Date <= end).ToArray();

        var performance = Performance.CalculatePerformance(boundedNavsDesc, latest, interval);

        if (performance == null)
        {
            return;
        }

        Symbol = boundedNavsDesc.First().Symbol;
        NavList = boundedNavsDesc.Reverse().ToArray();
        NavChange = performance.RawValueChange;
        NavChangePercentage = performance.Value;
    }
}
