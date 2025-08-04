using Pi.SetMarketData.Application.Interfaces.Holiday;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Helper;

public static class RankingHelper
{
    public static List<Instrument> Rank(List<RankingItem> rankingItems, List<Instrument> instruments)
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

    public async static Task<DateTime> CalculateRankingStartDate(DateTime currentDateUTC, string marketStartTimeBangkok, IHolidayApiQuery holidayApiQuery)
    {
        // Parse the market start time
        if (!TimeSpan.TryParse(marketStartTimeBangkok, out TimeSpan marketStartTimeSpan))
        {
            throw new ArgumentException("Invalid market start time format", nameof(marketStartTimeBangkok));
        }

        // Get previous day if holiday
        while (await holidayApiQuery.IsNotBusinessDays(currentDateUTC))
        {
            currentDateUTC = currentDateUTC.AddDays(-1);
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