using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketTimelineRendered;

public static class MarketTimelineRenderedService
{
    public static MarketTimelineRenderedResponse GetResult
    (
        Instrument instrument, 
        List<Domain.Entities.Intermission> intermissions, 
        List<CandleData> candleData, 
        ExchangeTimezone exchangeTimezone
    )
    {
        var today = DateTime.Today;
        var convertedIntermissions = intermissions.Select(intermission =>
        {
            var from = (long)0;
            var to = (long)0;
            
            if (intermission.From is TimeOnly fromTime)
            {
                var dateTimeFrom = new DateTime(today.Year, today.Month, today.Day,
                    fromTime.Hour, fromTime.Minute, fromTime.Second, DateTimeKind.Utc);
                dateTimeFrom = dateTimeFrom.AddHours(-7);    // GMT+7
                from = ((DateTimeOffset)dateTimeFrom).ToUnixTimeSeconds();
            }
            
            if (intermission.To is TimeOnly toTime)
            {
                var dateTimeTo = new DateTime(today.Year, today.Month, today.Day,
                    toTime.Hour, toTime.Minute, toTime.Second, DateTimeKind.Utc);
                dateTimeTo = dateTimeTo.AddHours(-7);        // GMT+7
                to = ((DateTimeOffset)dateTimeTo).ToUnixTimeSeconds();
            }

            return new Domain.Models.Response.Intermission
            {
                From = (int)from,
                To = (int)to
            };
        }).ToList();

        return new MarketTimelineRenderedResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new TimelineRenderedResponse
            {
                Venue = instrument?.Venue ?? "",
                Symbol = instrument?.Symbol ?? "",
                Data = candleData
                    .OrderBy(candle => ((DateTimeOffset)candle.Date).ToUnixTimeSeconds())
                    .Select(candle => new object[]
                    {
                        ((DateTimeOffset)candle.Date).ToUnixTimeSeconds(),
                        candle.Open.ToString(),
                        candle.High.ToString(),
                        candle.Low.ToString(),
                        candle.Close.ToString(),
                        candle.Volume.ToString()
                    }).ToList(),
                Intermissions = convertedIntermissions,
                Meta = new TimelineMeta
                {
                    Exchange = exchangeTimezone?.Exchange ?? "",
                    ExchangeTimezone = exchangeTimezone?.Timezone ?? "",
                    Country = exchangeTimezone?.Country ?? "",
                    OffsetSeconds = DataManipulation.GetOffset(exchangeTimezone?.Timezone ?? "")
                }
            }
        };
    }
}
