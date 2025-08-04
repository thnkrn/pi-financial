using System.Globalization;
using Pi.SetMarketDataWSS.Application.Interfaces.OrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchOrderBookMapperService : IOrderBookMapper
{
    private int _maximumLevel;
    public OrderBook Map(MarketByPriceLevelWrapper? message, OrderBook? orderBook)
    {
        if (message == null)
            return new OrderBook();

        _maximumLevel = message.MaximumLevel.Value;

        var orderBookValue = new OrderBook();
        var bidArray = InitializeBidAskArray(orderBook?.Bid);
        var askArray = InitializeBidAskArray(orderBook?.Offer);

        ProcessPriceLevelUpdates(message, bidArray, askArray);
        PopulateOrderBook(message, orderBook, orderBookValue, bidArray, askArray);

        return orderBookValue;
    }

    private static List<BidAskLevelModel> InitializeBidAskArray(List<BidAsk>? bidAskList)
    {
        return bidAskList?.Select(b => new BidAskLevelModel(
            decimal.TryParse(b.Price, out var price) ? price : 0m,
            int.TryParse(b.Quantity, out var quantity) ? quantity : 0,
            0 // Default level, will be updated later
        )).ToList() ?? [];
    }

    private static void ProcessPriceLevelUpdates(MarketByPriceLevelWrapper message, List<BidAskLevelModel> bidArray,
        List<BidAskLevelModel> askArray)
    {
        if (message.PriceLevelUpdates == null || message.PriceLevelUpdates.Count == 0)
            throw new InvalidOperationException("Price level updates are missing.");

        foreach (var item in message.PriceLevelUpdates) ProcessSingleUpdate(item, bidArray, askArray);

        // Sort arrays based on price
        bidArray.Sort((a, b) => b.Price.CompareTo(a.Price));    // Descending
        askArray.Sort((a, b) => a.Price.CompareTo(b.Price));    // Ascending
    }

    private static void ProcessSingleUpdate(PriceLevelUpdate item, List<BidAskLevelModel> bidArray,
        List<BidAskLevelModel> askArray)
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

        switch (updateAction)
        {
            case "D": // Delete
                if (index >= 0 && index < targetArray.Count) targetArray.RemoveAt(index);
                break;
            case "C": // Change
                if (index >= 0 && index < targetArray.Count)
                    targetArray[index] =
                        new BidAskLevelModel(price.ToFloat(), Convert.ToInt64(quantity.Value), level.Value);
                else
                    targetArray.Add(new BidAskLevelModel(price.ToFloat(), Convert.ToInt64(quantity.Value),
                        level.Value));
                break;
            case "N": // New
                var newEntry = new BidAskLevelModel(price.ToFloat(), Convert.ToInt64(quantity.Value), level.Value);
                if (index >= 0 && index <= targetArray.Count)
                    targetArray.Insert(index, newEntry);
                else
                    targetArray.Add(newEntry);
                break;
            default:
                throw new ArgumentException($"Invalid update action: {updateAction}");
        }
    }

    private void PopulateOrderBook(MarketByPriceLevelWrapper message, OrderBook? orderBook, OrderBook orderBookValue,
        List<BidAskLevelModel> bidArray, List<BidAskLevelModel> askArray)
    {
        orderBookValue.OrderBookId = int.Parse(message.OrderBookID.Value.ToString());
        orderBookValue.BidPrice = bidArray.Count > 0
            ? bidArray.FirstOrDefault()?.Price.ToString(CultureInfo.InvariantCulture)
            : null;
        orderBookValue.BidQuantity = bidArray.Count > 0 ? bidArray.FirstOrDefault()?.Quantity.ToString() : null;
        orderBookValue.OfferPrice = askArray.Count > 0
            ? askArray.FirstOrDefault()?.Price.ToString(CultureInfo.InvariantCulture)
            : null;
        orderBookValue.OfferQuantity = askArray.Count > 0 ? askArray.FirstOrDefault()?.Quantity.ToString() : null;

        orderBookValue.Bid = ConvertToBidAskList(bidArray);
        orderBookValue.Offer = ConvertToBidAskList(askArray);

        orderBookValue.Symbol = orderBook?.Symbol;
        orderBookValue.InstrumentId = orderBook?.InstrumentId ?? 0;
    }

    private List<BidAsk> ConvertToBidAskList(List<BidAskLevelModel> array)
    {
        return array.Select(b => new BidAsk
        {
            Price = b.Price.ToString(CultureInfo.InvariantCulture),
            Quantity = b.Quantity.ToString()
        }).Take(_maximumLevel).ToList();
    }
}