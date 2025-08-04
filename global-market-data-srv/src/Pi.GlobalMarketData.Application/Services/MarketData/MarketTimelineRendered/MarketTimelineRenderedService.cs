using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketTimelineRendered;

public static class MarketTimelineRenderedService
{
    public static MarketTimelineRenderedResponse GetResult(
        GeInstrument instrument,
        ExchangeTimezone exchangeTimezone,
        List<CandleData> candleData
    )
    {
        return new MarketTimelineRenderedResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new TimelineRenderedResponse
            {
                Venue = instrument.Venue ?? "",
                Symbol = instrument.Symbol,
                Data = candleData
                    .OrderBy(candle => ((DateTimeOffset)candle.Date).ToUnixTimeSeconds())
                    .Select(candle => new object[]
                    {
                        ((DateTimeOffset)candle.Date).ToUnixTimeSeconds(),
                        candle.Open.ToString(),
                        candle.High.ToString(),
                        candle.Low.ToString(),
                        candle.Close.ToString(),
                        candle.Volume.ToString(),
                        candle.Amount.ToString()
                    }).ToList(),
                Intermissions = [], // currently not found in data document
                Meta = new TimelineMeta
                {
                    Exchange = exchangeTimezone?.Exchange ?? "",
                    ExchangeTimezone = exchangeTimezone?.Timezone ?? "",
                    Country = exchangeTimezone?.Country ?? "",
                    OffsetSeconds = DataManipulation.GetOffset(exchangeTimezone?.Timezone ?? ""),
                }
            }
        };
    }
}
