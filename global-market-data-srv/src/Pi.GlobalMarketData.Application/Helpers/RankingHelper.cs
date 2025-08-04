using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Helpers;

public static class RankingHelper
{
    public static List<GeInstrument> Rank(List<RankingItem> rankingItems, List<GeInstrument> instruments)
    {
        var rankingMap = rankingItems
            .GroupBy(r => new { r.Symbol, r.Venue })
            .ToDictionary(
                g => g.Key,
                g => g.Max(r => r.Amount)
            );

        var rankedInstruments = instruments
            .Select(instrument => new
            {
                Instrument = instrument,
                Rank = rankingMap.TryGetValue(
                    new { instrument.Symbol, instrument.Venue },
                    out decimal amount) ? amount : 0m // Default to 0 if not found
            })
            .OrderByDescending(x => x.Rank)
            .ThenBy(x => instruments.IndexOf(x.Instrument))
            .Select(x => x.Instrument)
            .ToList();

        return rankedInstruments;
    }

    public static DateTime CalculateRankingStartDate(DateTime currentDateUTC, string marketStartTimeBangkok)
    {
        // Parse the market start time
        if (!TimeSpan.TryParse(marketStartTimeBangkok, out TimeSpan marketStartTimeSpan))
        {
            throw new ArgumentException("Invalid market start time format", nameof(marketStartTimeBangkok));
        }

        var bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        var compareDate = new DateTime(currentDateUTC.Year, currentDateUTC.Month, currentDateUTC.Day, marketStartTimeSpan.Hours, marketStartTimeSpan.Minutes, marketStartTimeSpan.Seconds);
        var compareDateUTC = TimeZoneInfo.ConvertTimeToUtc(compareDate, bangkokTimeZone);

        if (currentDateUTC > compareDateUTC)
        {
            return new DateTime(currentDateUTC.Year, currentDateUTC.Month, currentDateUTC.Day, compareDateUTC.Hour, compareDateUTC.Minute, compareDateUTC.Second, DateTimeKind.Utc);
        }

        if (currentDateUTC.DayOfWeek == DayOfWeek.Monday)
        {
            currentDateUTC = currentDateUTC.AddDays(-3);
        }
        else
        {
            currentDateUTC = currentDateUTC.AddDays(-1);
        }
        return new DateTime(currentDateUTC.Year, currentDateUTC.Month, currentDateUTC.Day, compareDateUTC.Hour, compareDateUTC.Minute, compareDateUTC.Second, DateTimeKind.Utc);
    }
}