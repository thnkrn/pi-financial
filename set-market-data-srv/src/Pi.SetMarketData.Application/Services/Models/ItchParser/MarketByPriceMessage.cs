using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class MarketByPriceParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookID { get; set; }
    public Numeric8 MaximumLevel { get; set; }
    public List<PriceLevelUpdateItemParams> PriceLevelUpdates { get; set; }
}

public class PriceLevelUpdateItemParams
{
    public Alpha UpdateAction { get; set; }
    public Alpha Side { get; set; }
    public Numeric8 Level { get; set; }
    public Price32 Price { get; set; }
    public Numeric64 Quantity { get; set; }
    public Numeric8 NumberOfDeletes { get; set; }
}

public class MarketByPriceMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Numeric32 OrderBookID { get; private set; }
    public Numeric8 MaximumLevel { get; private set; }
    public Numeric8 NumberOfLevelItems { get; private set; }
    public List<PriceLevelUpdateItem> PriceLevelUpdates { get; private set; }

    public MarketByPriceMessage(MarketByPriceParams marketByPriceParams)
    {
        MsgType = ItchMessageType.b; // 'b' – Market by Price Message.
        Nanos = marketByPriceParams.Nanos;
        OrderBookID = marketByPriceParams.OrderBookID;
        MaximumLevel = marketByPriceParams.MaximumLevel;
        NumberOfLevelItems = new Numeric8((byte)marketByPriceParams.PriceLevelUpdates.Count);
        PriceLevelUpdates = new List<PriceLevelUpdateItem>();

        foreach (var itemParams in marketByPriceParams.PriceLevelUpdates)
        {
            PriceLevelUpdates.Add(new PriceLevelUpdateItem(itemParams));
        }
    }

    public static MarketByPriceMessage Parse(byte[] bytes)
    {
        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var orderBookID = reader.ReadNumeric32();
            var maximumLevel = reader.ReadNumeric8();
            var numberOfLevelItems = reader.ReadNumeric8();

            var priceLevelUpdatesParams = new List<PriceLevelUpdateItemParams>();
            for (int i = 0; i < numberOfLevelItems.Value; i++)
            {
                var updateAction = reader.ReadAlpha(1);
                var side = reader.ReadAlpha(1);
                var level = reader.ReadNumeric8();
                var price = reader.ReadPrice32();
                var quantity = reader.ReadNumeric64();
                var numberOfDeletes = reader.ReadNumeric8();

                priceLevelUpdatesParams.Add(
                    new PriceLevelUpdateItemParams
                    {
                        UpdateAction = updateAction,
                        Side = side,
                        Level = level,
                        Price = price,
                        Quantity = quantity,
                        NumberOfDeletes = numberOfDeletes
                    }
                );
            }

            var marketByPriceParams = new MarketByPriceParams
            {
                Nanos = nanos,
                OrderBookID = orderBookID,
                MaximumLevel = maximumLevel,
                PriceLevelUpdates = priceLevelUpdatesParams
            };

            return new MarketByPriceMessage(marketByPriceParams);
        }
    }

    public override string ToString()
    {
        var messageStr =
            $"MarketByPriceMessage:\n"
            + $"Nanos: {Nanos},\n"
            + $"OrderBookID: {OrderBookID},\n"
            + $"MaximumLevel: {MaximumLevel},\n"
            + $"NumberOfLevelItems: {NumberOfLevelItems}\n"
            + $"PriceLevelUpdates:";
        foreach (var item in PriceLevelUpdates)
        {
            messageStr +=
                $"\n  UpdateAction: {item.UpdateAction}, Side: {item.Side}, "
                + $"Level: {item.Level}, Price: {item.Price}, Quantity: {item.Quantity}, "
                + $"NumberOfDeletes: {item.NumberOfDeletes}";
        }

        return messageStr;
    }
}

public class PriceLevelUpdateItem
{
    public Alpha UpdateAction { get; private set; }
    public Alpha Side { get; private set; }
    public Numeric8 Level { get; private set; }
    public Price32 Price { get; private set; }
    public Numeric64 Quantity { get; private set; }
    public Numeric8 NumberOfDeletes { get; private set; }

    public PriceLevelUpdateItem(PriceLevelUpdateItemParams priceLevelParams)
    {
        UpdateAction = priceLevelParams.UpdateAction;
        Side = priceLevelParams.Side;
        Level = priceLevelParams.Level;
        Price = priceLevelParams.Price;
        Quantity = priceLevelParams.Quantity;
        NumberOfDeletes = priceLevelParams.NumberOfDeletes;
    }
}
