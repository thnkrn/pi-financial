using System.Globalization;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchOrderBookMapperServiceBackup
{
    public OrderBook Map(MarketByPriceLevelWrapper? message, OrderBook? cacheValue)
    {
        var orderBookValue = new OrderBook();
        var bidArray =
            cacheValue?.Bid?.Select(b => new BidAskModel(decimal.Parse(b.Price), int.Parse(b.Quantity))).ToList() ?? [];
        var askArray =
            cacheValue?.Offer?.Select(a => new BidAskModel(decimal.Parse(a.Price), int.Parse(a.Quantity))).ToList() ??
            [];
        var unused = message?.MaximumLevel.Value ?? 0;

        if (message?.PriceLevelUpdates == null || message.PriceLevelUpdates.Count == 0)
            throw new InvalidOperationException("Price level updates are missing.");

        foreach (var item in message.PriceLevelUpdates)
        {
            var updateAction = item.UpdateAction.Value;
            var side = item.Side.Value;
            var level = item.Level;
            var price = item.Price;
            var quantity = item.Quantity;

            if (updateAction == null || side == null)
                throw new InvalidOperationException("Incomplete price level update data.");

            var index = level.Value - 1;
            var targetArray = side == "B" ? bidArray : askArray;

            // TODO: Review this logic
            // if (index < 0 || index > targetArray.Count) // Added: Validate index range
            //     throw new ArgumentOutOfRangeException(nameof(level),
            //         $"Level {index + 1} is out of valid range for {side} side.");

            switch (updateAction)
            {
                case "D":
                    if (index < targetArray.Count) targetArray.RemoveAt(index);
                    break;
                case "C":
                    if (index < targetArray.Count)
                        targetArray[index] = new BidAskModel(price.ToFloat(), Convert.ToInt32(quantity.Value));
                    break;
                case "N":
                    if (index <= targetArray.Count)
                        targetArray.Insert(index, new BidAskModel(price.ToFloat(), Convert.ToInt32(quantity.Value)));
                    break;
                default:
                    throw new ArgumentException("Invalid update action.", nameof(updateAction));
            }
        }

        orderBookValue.OrderBookId = int.Parse(message.OrderBookID.Value.ToString());

        // Use this logic for prevent throwing error on accessing first element
        orderBookValue.BidPrice =
            bidArray.Count > 0 ? bidArray.First().Price.ToString(CultureInfo.InvariantCulture) : null;
        orderBookValue.BidQuantity = bidArray.Count > 0 ? bidArray.First().Quantity.ToString() : null;
        orderBookValue.OfferPrice =
            askArray.Count > 0 ? askArray.First().Price.ToString(CultureInfo.InvariantCulture) : null;
        orderBookValue.OfferQuantity = askArray.Count > 0 ? askArray.First().Quantity.ToString() : null;

        orderBookValue.Bid = bidArray.Select(b => new BidAsk
        {
            Price = b.Price.ToString(CultureInfo.InvariantCulture),
            Quantity = b.Quantity.ToString()
        }).ToList();

        orderBookValue.Offer = askArray.Select(a => new BidAsk
        {
            Price = a.Price.ToString(CultureInfo.InvariantCulture),
            Quantity = a.Quantity.ToString()
        }).ToList();

        orderBookValue.Symbol = cacheValue?.Symbol;
        orderBookValue.InstrumentId = cacheValue?.InstrumentId ?? 0;

        return orderBookValue;
    }
}