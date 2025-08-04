using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class MarketByPriceParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookID { get; init; }
    public required Numeric8 MaximumLevel { get; init; }
    public required IReadOnlyList<PriceLevelUpdateItemParams> PriceLevelUpdates { get; init; }
}

public class PriceLevelUpdateItemParams
{
    public required Alpha UpdateAction { get; init; }
    public required Alpha Side { get; init; }
    public required Numeric8 Level { get; init; }
    public required Price32 Price { get; init; }
    public required Numeric64 Quantity { get; init; }
    public required Numeric8 NumberOfDeletes { get; init; }
}

public class MarketByPriceMessage : ItchMessage
{
    public MarketByPriceMessage(MarketByPriceParams marketByPriceParams)
    {
        MsgType = ItchMessageType.b; // 'b' – Market by Price Message.
        Nanos = marketByPriceParams.Nanos;
        OrderBookID = marketByPriceParams.OrderBookID;
        MaximumLevel = marketByPriceParams.MaximumLevel;
        NumberOfLevelItems = new Numeric8((byte)marketByPriceParams.PriceLevelUpdates.Count);
        PriceLevelUpdates = marketByPriceParams.PriceLevelUpdates
            .Select(itemParams => new PriceLevelUpdateItem(itemParams))
            .ToList()
            .AsReadOnly();
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookID { get; }
    public Numeric8 MaximumLevel { get; }
    public Numeric8 NumberOfLevelItems { get; }
    public IReadOnlyList<PriceLevelUpdateItem> PriceLevelUpdates { get; }

    public static MarketByPriceMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var maximumLevel = reader.ReadNumeric8();
        var numberOfLevelItems = reader.ReadNumeric8();

        var priceLevelUpdatesParams = new List<PriceLevelUpdateItemParams>();
        for (var i = 0; i < numberOfLevelItems.Value; i++)
            priceLevelUpdatesParams.Add(new PriceLevelUpdateItemParams
            {
                UpdateAction = reader.ReadAlpha(1),
                Side = reader.ReadAlpha(1),
                Level = reader.ReadNumeric8(),
                Price = reader.ReadPrice32(),
                Quantity = reader.ReadNumeric64(),
                NumberOfDeletes = reader.ReadNumeric8()
            });

        return new MarketByPriceMessage(new MarketByPriceParams
        {
            Nanos = nanos,
            OrderBookID = orderBookId,
            MaximumLevel = maximumLevel,
            PriceLevelUpdates = priceLevelUpdatesParams
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                MarketByPriceMessage:
                Nanos: {Nanos},
                OrderBookID: {OrderBookID},
                MaximumLevel: {MaximumLevel},
                NumberOfLevelItems: {NumberOfLevelItems}
                PriceLevelUpdates:
                {string.Join("\n", PriceLevelUpdates.Select(item => $"  {item}"))}
                """;
    }
}

public class PriceLevelUpdateItem
{
    public PriceLevelUpdateItem(PriceLevelUpdateItemParams priceLevelParams)
    {
        UpdateAction = priceLevelParams.UpdateAction;
        Side = priceLevelParams.Side;
        Level = priceLevelParams.Level;
        Price = priceLevelParams.Price;
        Quantity = priceLevelParams.Quantity;
        NumberOfDeletes = priceLevelParams.NumberOfDeletes;
    }

    public Alpha UpdateAction { get; }
    public Alpha Side { get; }
    public Numeric8 Level { get; }
    public Price32 Price { get; }
    public Numeric64 Quantity { get; }
    public Numeric8 NumberOfDeletes { get; }

    public override string ToString()
    {
        return
            $"UpdateAction: {UpdateAction}, Side: {Side}, Level: {Level}, Price: {Price}, Quantity: {Quantity}, NumberOfDeletes: {NumberOfDeletes}";
    }
}