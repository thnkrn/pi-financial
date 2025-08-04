using System.Globalization;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Helpers;
using Pi.GlobalMarketDataWSS.Application.Interfaces.FixMapper;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.Application.Services.FixMapper;

public class PriceInfoMapperService : IPriceInfoMapperService
{
    public StreamingBody Map(StreamingBody streamingBody, string marketSession, Entry entry)
    {
        streamingBody.PreClose = string.IsNullOrEmpty(streamingBody.PreClose) ? "0.00" : streamingBody.PreClose;

        MapEntry(streamingBody, marketSession, entry);

        streamingBody.IsProjected = MarketSession.PreMarket.Equals(marketSession, StringComparison.OrdinalIgnoreCase);

        if (MarketSession.MainSession.Equals(marketSession, StringComparison.OrdinalIgnoreCase) &&
            MarketSession.PreMarket.Equals(streamingBody.Status, StringComparison.OrdinalIgnoreCase))
        {
            // Moment that market change from Premarket to MainSession
            streamingBody.High24H = streamingBody.Price;
            streamingBody.Low24H = streamingBody.Price;
        }

        streamingBody.Status = !string.IsNullOrEmpty(marketSession) ? marketSession : streamingBody.Status;
        streamingBody.IsProjected =
            string.Equals(marketSession, MarketSession.PreMarket, StringComparison.OrdinalIgnoreCase);
        streamingBody.InstrumentType = InstrumentType.GlobalEquity;
        streamingBody.SecurityType = InstrumentType.GlobalEquity;
        streamingBody.LastPriceTime = entry.MdEntryTime.ToUnixMillisecondTimestamp();
        streamingBody.TotalAmount = CalculateTotalAmount(streamingBody.Price, streamingBody.TotalVolume);
        streamingBody.TotalAmountK = CalculateTotalAmount(streamingBody.Price, streamingBody.TotalVolumeK);

        return streamingBody;
    }

    private static void MapEntry(StreamingBody streamingBody, string marketSession, Entry entry)
    {
        // Map fix entry data into PriceInfo
        switch (entry.MdEntryType)
        {
            case FixMessageType.Trade:
                streamingBody = HandleTradeMessage(streamingBody, marketSession, entry);
                break;
            case FixMessageType.OpeningPrice:
                streamingBody.Open = entry.MdEntryPx.ToString(CultureInfo.InvariantCulture);
                break;
            case FixMessageType.ClosingPrice:
                streamingBody = HandleClosePriceMessage(streamingBody, entry);
                break;
            case FixMessageType.B:
                streamingBody.TotalVolume = entry.MdEntrySize.ToString() ?? "0.00";
                streamingBody.TotalVolumeK = (entry.MdEntrySize / 1000).ToString() ?? "0.00";
                break;
        }

        // If marketStatus is MainSession PreClose should be PriorClose (Price from msgType: 5)
        if (MarketSession.MainSession.Equals(marketSession, StringComparison.OrdinalIgnoreCase))
            streamingBody.PreClose = streamingBody.PriorClose;
    }

    private static StreamingBody HandleTradeMessage(StreamingBody streamingBody, string marketSession, Entry entry)
    {
        if (!MarketSession.MainSession.Equals(marketSession, StringComparison.OrdinalIgnoreCase))
        {
            if (streamingBody.Venue != GlobalMarketVenue.HKEX)
            {
                // MarketStatus is not MainSession
                streamingBody.AuctionPrice = decimal.Round(entry.MdEntryPx, 2, MidpointRounding.ToZero).ToString("F2", CultureInfo.InvariantCulture);
                streamingBody.AuctionVolume = entry.MdEntrySize.ToString() ?? "0.00";
            }
            else
            {
                HandleStreamingBody(streamingBody, entry);
            }
        }
        else
        {
            HandleStreamingBody(streamingBody, entry);
        }
        
        return streamingBody;
    }
    
    private static StreamingBody HandleClosePriceMessage(StreamingBody streamingBody, Entry entry)
    {
        streamingBody.Price =  decimal.Round(entry.MdEntryPx, 2, MidpointRounding.ToZero).ToString("F2", CultureInfo.InvariantCulture);
        streamingBody.PriceChanged = CalculatePriceChanged(entry.MdEntryPx, streamingBody.PreClose);
        streamingBody.PriceChangedRate = CalculatePriceChangedRate(entry.MdEntryPx, streamingBody.PreClose);
        
        return streamingBody;
    }

    private static void HandleStreamingBody(StreamingBody streamingBody, Entry entry)
    {
        // MarketStatus is MainSession
        streamingBody.Price =  decimal.Round(entry.MdEntryPx, 2, MidpointRounding.ToZero).ToString("F2", CultureInfo.InvariantCulture);
        streamingBody.Volume = entry.MdEntrySize.ToString() ?? "0.00";
        streamingBody.Amount =
            (entry.MdEntryPx * entry.MdEntrySize ?? 0).ToString(CultureInfo.InvariantCulture);
        streamingBody.PriceChanged = CalculatePriceChanged(entry.MdEntryPx, streamingBody.PreClose);
        streamingBody.PriceChangedRate = CalculatePriceChangedRate(entry.MdEntryPx, streamingBody.PreClose);
        // Update High24H
        if (!decimal.TryParse(streamingBody.High24H, out var currentHigh24H) ||
            entry.MdEntryPx > currentHigh24H) streamingBody.High24H = streamingBody.Price;

        // Update Low24H
        if (!decimal.TryParse(streamingBody.Low24H, out var currentLow24H) ||
            entry.MdEntryPx < currentLow24H) streamingBody.Low24H = streamingBody.Price;

        if (!MarketSession.MainSession.Equals(streamingBody.Status, StringComparison.OrdinalIgnoreCase))
        {
            // Market change from 
            streamingBody.Low24H = streamingBody.Price;
            streamingBody.High24H = streamingBody.Price;
        }
    }

    private static string CalculatePriceChanged(decimal currentPrice, string closedPriceStr)
    {
        if (decimal.TryParse(closedPriceStr, out var closedPrice))
        {
            var result = Math.Round(currentPrice - closedPrice, 2);

            return result.ToString("F2");
        }

        return "0.00";
    }

    private static string CalculatePriceChangedRate(decimal currentPrice, string closedPriceStr)
    {
        if (decimal.TryParse(closedPriceStr, out var closedPrice) && closedPrice > 0)
        {
            var result = Math.Round((currentPrice - closedPrice) / closedPrice * 100, 2);

            return result.ToString("F2");
        }

        return "0.00";
    }

    private static string CalculateTotalAmount(string price, string? totalVolume)
    {
        if (string.IsNullOrEmpty(price) || string.IsNullOrEmpty(totalVolume))
            return "0.00";

        if (
            double.TryParse(price, out var priceValue)
            && double.TryParse(totalVolume, out var totalVolumeValue)
        )
            return (priceValue * totalVolumeValue).ToString("F2");

        return "0.00";
    }

    public static StreamingBody MapForCache(StreamingBody streamingBody, PriceInfo priceInfo, string marketSession)
    {
        throw new NotImplementedException();
    }
}
