using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class EquilibriumPriceParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookID { get; set; }
    public Numeric64 BidQuantity { get; set; }
    public Numeric64 AskQuantity { get; set; }
    public Price32 EquilibriumPrice { get; set; }
}

public class EquilibriumPriceMessage : ItchMessage
{
    public EquilibriumPriceMessage(EquilibriumPriceParams equilibriumPriceParams)
    {
        MsgType = 'Z'; // 'Z' – Equilibrium Price Message.
        Nanos = equilibriumPriceParams.Nanos;
        OrderBookID = equilibriumPriceParams.OrderBookID;
        BidQuantity = equilibriumPriceParams.BidQuantity;
        AskQuantity = equilibriumPriceParams.AskQuantity;
        EquilibriumPrice = equilibriumPriceParams.EquilibriumPrice;
    }

    public Numeric32 Nanos { get; }
    public Numeric32 OrderBookID { get; }
    public Numeric64 BidQuantity { get; }
    public Numeric64 AskQuantity { get; }
    public Price32 EquilibriumPrice { get; }

    public static EquilibriumPriceMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 28)
            throw new ArgumentException("Invalid data format for EquilibriumPriceMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var orderBookID = reader.ReadNumeric32();
            var bidQuantity = reader.ReadNumeric64();
            var askQuantity = reader.ReadNumeric64();
            var equilibriumPrice = reader.ReadPrice32();

            var equilibriumPriceParams = new EquilibriumPriceParams
            {
                Nanos = nanos,
                OrderBookID = orderBookID,
                BidQuantity = bidQuantity,
                AskQuantity = askQuantity,
                EquilibriumPrice = equilibriumPrice
            };

            return new EquilibriumPriceMessage(equilibriumPriceParams);
        }
    }

    public string ToStringUnitTest()
    {
        return $"EquilibriumPriceMessage:\n"
               + $"MsgType: {MsgType},\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookID: {OrderBookID},\n"
               + $"BidQuantity: {BidQuantity},\n"
               + $"AskQuantity: {AskQuantity},\n"
               + $"EquilibriumPrice: {EquilibriumPrice}";
    }
}