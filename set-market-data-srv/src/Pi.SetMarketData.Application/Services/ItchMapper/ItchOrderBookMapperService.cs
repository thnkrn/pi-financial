using System.Globalization;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Services.ItchMapper;

public class ItchOrderBookMapperService : IItchOrderBookMapperService
{
    public OrderBook? Map(MarketByPriceLevelWrapper? message, OrderBook? storedData)
    {
        if (message == null)
            return null;

        var orderBookValue = new OrderBook();
        var bidArray =
            storedData?.Bid.Select(b => new BidAskModel(decimal.Parse(b.Price), int.Parse(b.Quantity))).ToList() ??
            [];
        var askArray =
            storedData?.Offer.Select(a => new BidAskModel(decimal.Parse(a.Price), int.Parse(a.Quantity))).ToList() ??
            [];

        if (message.PriceLevelUpdates == null || message.PriceLevelUpdates.Count == 0)
            throw new InvalidOperationException("Price level updates are missing.");

        ProcessBidAskMessage(message, bidArray, askArray);

        orderBookValue.OrderBookId = int.Parse(message.OrderBookID.Value.ToString());

        // Use this logic for prevent throwing error on accessing first element
        orderBookValue.BidPrice = bidArray.Count > 0
            ? bidArray.FirstOrDefault()?.Price.ToString(CultureInfo.InvariantCulture)
            : null;
        orderBookValue.BidQuantity = bidArray.Count > 0 ? bidArray.FirstOrDefault()?.Quantity.ToString() : null;
        orderBookValue.OfferPrice = askArray.Count > 0
            ? askArray.FirstOrDefault()?.Price.ToString(CultureInfo.InvariantCulture)
            : null;
        orderBookValue.OfferQuantity = askArray.Count > 0 ? askArray.FirstOrDefault()?.Quantity.ToString() : null;

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

        orderBookValue.Symbol = storedData?.Symbol;
        orderBookValue.InstrumentId = storedData?.InstrumentId ?? 0;

        return orderBookValue;
    }

    private static void ProcessBidAskMessage(MarketByPriceLevelWrapper message, List<BidAskModel> bidArray,
        List<BidAskModel> askArray)
    {
        foreach (var t in message.PriceLevelUpdates)
        {
            var updateAction = t.UpdateAction.Value;
            var side = t.Side.Value;
            var level = t.Level;
            var price = t.Price;
            var quantity = t.Quantity;

            if (updateAction == null || side == null)
                throw new InvalidOperationException("Incomplete price level update data.");

            var index = level.Value - 1;
            var targetArray = side == "B" ? bidArray : askArray;

            ProcessUpdateAction(updateAction, index, targetArray, price, quantity);
        }
    }

    private static void ProcessUpdateAction(string updateAction, int index, List<BidAskModel> targetArray,
        Price64 price,
        Numeric64 quantity)
    {
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
                throw new InvalidOperationException($"Invalid update action: {nameof(updateAction)}.");
        }
    }
}