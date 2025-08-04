using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Fix;

namespace Pi.GlobalMarketData.Application.Services.FixMapper;

public static class FixMapperService
{
    public static RealtimeMarketData? MapToRealtimeMarketData(Entry entry, string? symbol, string? venue)
    {
        if (entry.MDEntryTime.HasValue)
            try
            {
                DateTimeOffset entryDate = new(entry.MDEntryTime.Value, TimeSpan.Zero);

                var price = entry.MDEntryPx;
                int? volume = int.Parse(entry.MDEntrySize.ToString() ?? "0");
                var amount = entry.MDEntrySize * entry.MDEntryPx;

                return new RealtimeMarketData
                {
                    DateTime = entryDate,
                    Symbol = symbol,
                    Venue = venue,
                    Price = price,
                    Volume = volume,
                    Amount = amount
                };
            }
            catch (Exception)
            {
                return null;
            }

        return null;
    }
}