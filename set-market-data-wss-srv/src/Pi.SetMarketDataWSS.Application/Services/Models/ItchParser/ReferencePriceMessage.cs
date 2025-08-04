using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class ReferencePriceParams
{
    // Nanoseconds portion of the timestamp.
    public Numeric32 Nanos { get; set; }

    // Orderbook ID.
    public Numeric32 OrderbookId { get; set; }

    // Type of price published.
    public Numeric8 PriceType { get; set; }

    // Price based on Price Type.
    public Price32 Price { get; set; }

    // Timestamp when the Price was updated.
    public Timestamp UpdatedTimestamp { get; set; }
}

public class ReferencePriceMessage : ItchMessage
{
    public ReferencePriceMessage(ReferencePriceParams referencePriceParams)
    {
        MsgType = 'Q'; // 'Q' – Reference Price Message
        Nanos = referencePriceParams.Nanos;
        OrderbookId = referencePriceParams.OrderbookId;
        PriceType = referencePriceParams.PriceType;
        Price = referencePriceParams.Price;
        UpdatedTimestamp = referencePriceParams.UpdatedTimestamp;
    }

    public Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Numeric8 PriceType { get; }
    public Price32 Price { get; }
    public Timestamp UpdatedTimestamp { get; }

    public static ReferencePriceMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 21) // Ensure byte array is of expected length
            throw new ArgumentException("Invalid data format for ReferencePriceMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            return new ReferencePriceMessage(
                new ReferencePriceParams
                {
                    Nanos = reader.ReadNumeric32(),
                    OrderbookId = reader.ReadNumeric32(),
                    PriceType = reader.ReadNumeric8(),
                    Price = reader.ReadPrice32(),
                    UpdatedTimestamp = reader.ReadTimestamp()
                }
            );
        }
    }

    public string ToStringUnitTest()
    {
        return $"ReferencePriceMessage:\n"
               + $"MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, PriceType: {PriceType}, "
               + $"Price: {Price}, UpdatedTimestamp: {UpdatedTimestamp}";
    }
}