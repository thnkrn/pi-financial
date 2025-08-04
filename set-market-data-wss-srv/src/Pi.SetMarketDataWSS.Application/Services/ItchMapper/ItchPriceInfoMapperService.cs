using System.Diagnostics.CodeAnalysis;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchPriceInfoMapperService : IItchPriceInfoMapperService
{
    private static readonly HashSet<string> PreOpen = ["pre-open1", "pre-open2"];

    [SuppressMessage("SonarQube", "S3776")]
    public PriceInfo Map(ItchMessage message, object? cachedPriceInfo, object? cachedPriceInfoQUpper,
        string orderBookStateStatus = "")
    {
        if (message == null)
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        var priceInfo = cachedPriceInfo != null ? (PriceInfo)cachedPriceInfo : new PriceInfo();
        var priceInfoQUpper = cachedPriceInfoQUpper != null ? (PriceInfo)cachedPriceInfoQUpper : new PriceInfo();

        if (orderBookStateStatus.Contains("RESET_STAT", StringComparison.InvariantCultureIgnoreCase) ||
            orderBookStateStatus.Contains("Closed", StringComparison.InvariantCultureIgnoreCase))
        {
            return priceInfo;
        }

        switch (message)
        {
            case TradeTickerMessageWrapper tradeTickerMessage:
                // Map from MsgType : i
                priceInfo.Price = tradeTickerMessage.Price.Value.ToString();
                priceInfo.PriceChanged = CalculatePriceChanged(priceInfo.Price, priceInfoQUpper.PreClose).ToString();
                priceInfo.PriceChangedRate =
                    CalculatePriceChangeRate(priceInfo.Price, priceInfoQUpper.PreClose).ToString("0.00");
                priceInfo.CalculatedPriceChangedRate = true;
                priceInfo.Volume = tradeTickerMessage.Quantity.Value.ToString();
                priceInfo.Aggressor = tradeTickerMessage.Aggressor?.Value;
                priceInfo.LastPriceTime =
                    Timestamp.ConvertToUnixTimestampMilliseconds(tradeTickerMessage.DealDateTime.ToString());
                priceInfo.Amount =
                    ((ulong)tradeTickerMessage.Price.Value * tradeTickerMessage.Quantity.Value).ToString();

                switch (priceInfo.Aggressor)
                {
                    case "B":
                        var lastAverageSell =
                            decimal.Parse(priceInfo.AverageSell ?? tradeTickerMessage.Price.Value.ToString());
                        var averageSell = (lastAverageSell + tradeTickerMessage.Price.Value) / 2;
                        priceInfo.AverageSell = averageSell.ToString("F0");
                        break;
                    case "A":
                        var lastAverageBuy =
                            decimal.Parse(priceInfo.AverageBuy ?? tradeTickerMessage.Price.Value.ToString());
                        var averageBuy = (lastAverageBuy + tradeTickerMessage.Price.Value) / 2;
                        priceInfo.AverageBuy = averageBuy.ToString("F0");
                        break;
                }

                break;
            case TradeStatisticsMessageWrapper tradeStatisticsMessage:
                // Map from MsgType : I
                priceInfo.AuctionPrice = tradeStatisticsMessage.LastAuctionPrice?.Value.ToString();
                priceInfo.AuctionVolume = tradeStatisticsMessage.TurnoverQuantity?.Value.ToString();
                priceInfo.Open = tradeStatisticsMessage.OpenPrice?.Value.ToString();
                priceInfo.High24H = tradeStatisticsMessage.HighPrice?.Value.ToString();
                priceInfo.Low24H = tradeStatisticsMessage.LowPrice?.Value.ToString();
                priceInfo.Average = tradeStatisticsMessage.AveragePrice?.Value.ToString();
                priceInfo.TotalAmount = tradeStatisticsMessage.TurnoverValue?.Value.ToString();
                priceInfo.TotalAmountK = (tradeStatisticsMessage.TurnoverValue?.Value / 1000).ToString();
                priceInfo.TotalVolume = tradeStatisticsMessage.TurnoverQuantity?.Value.ToString();
                priceInfo.TotalVolumeK = (tradeStatisticsMessage.TurnoverQuantity?.Value).ToString();

                if (orderBookStateStatus.Equals("open0", StringComparison.OrdinalIgnoreCase))
                    priceInfo.Open0 = tradeStatisticsMessage.LastAuctionPrice?.Value.ToString();
                if (orderBookStateStatus.Equals("open1", StringComparison.OrdinalIgnoreCase))
                    priceInfo.Open1 = tradeStatisticsMessage.LastAuctionPrice?.Value.ToString();
                if (orderBookStateStatus.Equals("open2", StringComparison.OrdinalIgnoreCase))
                    priceInfo.Open2 = tradeStatisticsMessage.LastAuctionPrice?.Value.ToString();

                break;
            case ReferencePriceMessageWrapper referencePriceMessage:
                // Map from MsgType : Q
                if (referencePriceMessage.PriceType == 11)
                {
                    priceInfo.Price = referencePriceMessage.Price.Value.ToString();
                    priceInfo.PriceChanged = CalculatePriceChanged(priceInfo.Price, priceInfo.PreClose).ToString();
                    priceInfo.PriceChangedRate =
                        CalculatePriceChangeRate(priceInfo.Price, priceInfo.PreClose).ToString("0.00");
                } else if (referencePriceMessage.PriceType == 6)
                {
                    priceInfo.PreClose = referencePriceMessage.Price.Value.ToString();
                }

                break;
            case IndexPriceMessageWrapper indexPriceMessage:
                // Map from MsgType : J
                var priceJUpper = indexPriceMessage.Value.Value;
                if (Price64.HasValue(priceJUpper)
                    && long.TryParse(priceJUpper.ToString(), out var parseJUpper) && parseJUpper > 0)
                    priceInfo.Price = parseJUpper.ToString();

                priceInfo.Open = indexPriceMessage.OpenValue.Value.ToString();
                priceInfo.High24H = indexPriceMessage.HighValue.Value.ToString();
                priceInfo.Low24H = indexPriceMessage.LowValue.Value.ToString();
                priceInfo.PriceChanged = indexPriceMessage.Change.Value.ToString();
                priceInfo.PriceChangedRate = indexPriceMessage.ChangePercent.Value.ToString();
                priceInfo.PreClose = indexPriceMessage.PreviousClose.Value.ToString();
                priceInfo.TotalAmount = indexPriceMessage.TradedValue.Value.ToString();
                priceInfo.TotalVolume = indexPriceMessage.TradedVolume.Value.ToString();
                priceInfo.TotalAmountK = ((ulong)indexPriceMessage.TradedValue.Value / 1000).ToString();
                priceInfo.TotalVolumeK = indexPriceMessage.TradedVolume.Value.ToString();
                break;
            case PriceLimitMessageWrapper priceLimitMessageWrapper:
                // Map from MsgType : k
                priceInfo.Ceiling = priceLimitMessageWrapper.UpperLimit?.Value.ToString();
                priceInfo.Floor = priceLimitMessageWrapper.LowerLimit?.Value.ToString();
                break;
            case EquilibriumPriceMessageWrapper equilibriumPriceMessage:
                // Map from MsgType : Z
                if (!PreOpen.Contains(orderBookStateStatus.ToLowerInvariant()))
                    break;

                var priceKUpper = equilibriumPriceMessage.EquilibriumPrice?.Value;
                if (Price64.HasValue(priceKUpper))
                {
                    if (long.TryParse(priceKUpper.ToString(), out var parseKUpper) && parseKUpper > 0)
                        priceInfo.Price = parseKUpper.ToString();

                    priceInfo.AuctionPrice = priceKUpper.ToString();
                }

                break;
        }

        return priceInfo;
    }

    private static int? CalculatePriceChanged(string currentPriceStr, string? closedPriceStr)
    {
        if (string.IsNullOrEmpty(currentPriceStr) || string.IsNullOrEmpty(closedPriceStr)) return null;

        if (int.TryParse(currentPriceStr, out var currentPrice) &&
            int.TryParse(closedPriceStr, out var closedPrice))
            return currentPrice - closedPrice;
        return null;
    }

    private static double CalculatePriceChangeRate(string currentPriceStr, string? closedPriceStr)
    {
        if (string.IsNullOrEmpty(currentPriceStr) || string.IsNullOrEmpty(closedPriceStr)) return 0.00;

        if (double.TryParse(currentPriceStr, out var currentPrice) &&
            double.TryParse(closedPriceStr, out var closedPrice) &&
            closedPrice > 0.0f) return (currentPrice - closedPrice) / closedPrice * 100;

        return 0.00;
    }
}
