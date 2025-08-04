using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketIndicator;

public static class MarketIndicatorService
{
    public static MarketIndicatorResponse GetResult(
        string candleType,
        MarketIndicatorRequest request,
        GeInstrument instrument,
        ExchangeTimezone exchangeTimezone,
        List<CandleData> candleList,
        List<TechnicalIndicators> indicatorList,
        DateTime firstCandle
    )
    {
        int firstCandleTime = (int)DataManipulation.ToUnixTimestamp(firstCandle);
        if (request.FromTimestamp > 0)
        {
            candleList.Reverse();
            indicatorList.Reverse();
        }

        List<List<object>> candles = [];

        if (candleList.Count > 0)
        {
            foreach (var candle in candleList)
            {
                var adjustedDate = candle.Date;
                if (candleType == CandleType.candle1Day)
                {
                    adjustedDate = adjustedDate.AddHours(4);
                    adjustedDate = DateTime.SpecifyKind(adjustedDate, DateTimeKind.Utc);
                }
                candles.Add(
                    [
                        DataManipulation.ToUnixTimestamp(adjustedDate),
                        candle.Open.ToString(),
                        candle.High.ToString(),
                        candle.Low.ToString(),
                        candle.Close.ToString(),
                        candle.Volume.ToString(),
                        candle.Amount.ToString()
                    ]
                );
            }
        }

        var boll = new List<object>();
        var ema = new List<object>();
        var kdj = new List<object>();
        var ma = new List<object>();
        var macd = new List<object>();
        var rsi = new List<object>();

        if (indicatorList != null && indicatorList.Count > 0)
        {
            foreach (var indicator in indicatorList)
            {
                if (indicator.DateTime == null)
                    continue;

                var timestamp = (int)DataManipulation.ToUnixTimestamp(indicator.DateTime.Value);

                boll.Add(
                    new object[]
                    {
                        timestamp,
                        indicator.BollUpper?.ToString() ?? "0",
                        indicator.BollMedium?.ToString() ?? "0",
                        indicator.BollLower?.ToString() ?? "0"
                    }
                );

                ema.Add(
                    new object[]
                    {
                        timestamp,
                        indicator.Ema10?.ToString() ?? "0",
                        indicator.Ema25?.ToString() ?? "0"
                    }
                );

                kdj.Add(
                    new object[]
                    {
                        timestamp,
                        indicator.KdjK?.ToString() ?? "0",
                        indicator.KdjD?.ToString() ?? "0",
                        indicator.KdjJ?.ToString() ?? "0"
                    }
                );

                ma.Add(
                    new object[]
                    {
                        timestamp,
                        indicator.Sma10?.ToString() ?? "0",
                        indicator.Sma25?.ToString() ?? "0"
                    }
                );

                macd.Add(
                    new object[]
                    {
                        timestamp,
                        indicator.MacdMacdDiff?.ToString() ?? "0",
                        indicator.MacdSignalDea?.ToString() ?? "0",
                        indicator.MacdOsc?.ToString() ?? "0"
                    }
                );

                rsi.Add(new object[] { timestamp, indicator.RsiRsi?.ToString() ?? "0" });
            }
        }

        return new MarketIndicatorResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new IndicatorResponse
            {
                Meta = new Meta
                {
                    Exchange = instrument.Venue ?? string.Empty,
                    ExchangeTimezone = exchangeTimezone.Timezone,
                    Country = exchangeTimezone.Country,
                    OffsetSeconds = DataManipulation.GetOffset(exchangeTimezone.Timezone),
                },
                Venue = instrument.Venue ?? string.Empty,
                Symbol = instrument.Symbol ?? string.Empty,
                CandleType = request.CandleType,
                Candles = candles,
                Indicators = request.CompleteTradingDay // if this flag is true, then the indicators will be null
                    ? null
                    : new TechnicalIndicatorsResponse
                    {
                        Boll = boll,
                        Ema = ema,
                        Kdj = kdj,
                        Ma = ma,
                        Macd = macd,
                        Rsi = rsi
                    },
                FirstCandleTime = firstCandleTime
            }
        };
    }
}
