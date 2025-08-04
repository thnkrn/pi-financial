using MassTransit.Futures.Contracts;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class Performance
{
    public Performance()
    {
    }

    public Performance(IEnumerable<HistoricalNav> navs)
    {
        HistoricalReturnPercentages = CalculatePercentages(navs).ToList();
    }

    public List<HistoricalReturnPercentage> HistoricalReturnPercentages { get; init; }
    public List<HistoricalReturnPercentage> AnnualizedHistoricalReturnPercentages { get; init; }
    public double? Yield1Y { get; init; }
    private const int FallbackDays = 7;

    private static IEnumerable<HistoricalReturnPercentage> CalculatePercentages(IEnumerable<HistoricalNav> navs)
    {
        navs = navs.OrderByDescending(q => q.Date).ToList();
        var latest = navs.FirstOrDefault();
        if (latest == null)
        {
            return [];
        }

        return Enum.GetValues<Interval>().Select(interval =>
        {
            var result = CalculatePerformance(navs, latest, interval);
            if (result == null)
            {
                return new HistoricalReturnPercentage()
                {
                    Interval = interval,
                    Value = null,
                };
            }

            return result;
        });
    }

    public static HistoricalReturnPercentage CalculatePerformance(IEnumerable<HistoricalNav> sortedNavsDesc, HistoricalNav latest, Interval interval)
    {
        HistoricalNav last;
        switch (interval)
        {
            case Interval.Over3Months:
            case Interval.Over6Months:
            case Interval.Over1Year:
            case Interval.Over3Years:
            case Interval.Over5Years:
            case Interval.YearToDate:
                (DateTime start, DateTime end) = interval.GetIntervalDateTimes(latest.Date);
                if (interval != Interval.YearToDate)
                {
                    end = start.AddDays(FallbackDays); // fallback for specific interval
                }

                var boundedNav = sortedNavsDesc.Where(x => x.Date >= start && x.Date <= end).ToList();
                last = boundedNav.LastOrDefault();
                break;
            case Interval.SinceInception:
                last = sortedNavsDesc.LastOrDefault();
                break;
            default:
                last = null;
                break;
        }


        if (last == null)
        {
            return null;
        }

        decimal change = latest.Nav - last.Nav;
        return new HistoricalReturnPercentage()
        {
            Interval = interval,
            Value = last.Nav is not 0 ? (double)((change * 100) / last.Nav) : 0,
            RawValueChange = change
        };
    }
}
