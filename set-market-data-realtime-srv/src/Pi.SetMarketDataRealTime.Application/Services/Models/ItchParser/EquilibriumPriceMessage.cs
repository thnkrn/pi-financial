using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class EquilibriumPriceParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookID { get; init; }
    public required Numeric64 BidQuantity { get; init; }
    public required Numeric64 AskQuantity { get; init; }
    public required Price32 EquilibriumPrice { get; init; }
}

public class EquilibriumPriceMessage : ItchMessage
{
    public EquilibriumPriceMessage(EquilibriumPriceParams equilibriumPriceParams)
    {
        MsgType = 'Z';
        Nanos = equilibriumPriceParams.Nanos;
        OrderBookID = equilibriumPriceParams.OrderBookID;
        BidQuantity = equilibriumPriceParams.BidQuantity;
        AskQuantity = equilibriumPriceParams.AskQuantity;
        EquilibriumPrice = equilibriumPriceParams.EquilibriumPrice;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookID { get; }
    public Numeric64 BidQuantity { get; }
    public Numeric64 AskQuantity { get; }
    public Price32 EquilibriumPrice { get; }

    public static EquilibriumPriceMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 28)
            throw new ArgumentException("Invalid data format for EquilibriumPriceMessage.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookID = reader.ReadNumeric32();
        var bidQuantity = reader.ReadNumeric64();
        var askQuantity = reader.ReadNumeric64();
        var equilibriumPrice = reader.ReadPrice32();

        return new EquilibriumPriceMessage(new EquilibriumPriceParams
        {
            Nanos = nanos,
            OrderBookID = orderBookID,
            BidQuantity = bidQuantity,
            AskQuantity = askQuantity,
            EquilibriumPrice = equilibriumPrice
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                EquilibriumPriceMessage:
                MsgType: {MsgType},
                Nanos: {Nanos},
                OrderBookID: {OrderBookID},
                BidQuantity: {BidQuantity},
                AskQuantity: {AskQuantity},
                EquilibriumPrice: {EquilibriumPrice}
                """;
    }
}