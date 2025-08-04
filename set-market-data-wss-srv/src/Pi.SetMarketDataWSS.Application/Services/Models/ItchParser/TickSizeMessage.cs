using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class TickSizeParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookId { get; set; }
    public Price64 TickSize { get; set; }
    public Price32 PriceFrom { get; set; }
    public Price32 PriceTo { get; set; }
}

public class TickSizeMessage : ItchMessage
{
    public TickSizeMessage(TickSizeParams tickSizeParams)
    {
        MsgType = ItchMessageType.L; // Message type for Tick Size Message
        Nanos = tickSizeParams.Nanos;
        OrderBookId = tickSizeParams.OrderBookId;
        TickSize = tickSizeParams.TickSize;
        PriceFrom = tickSizeParams.PriceFrom;
        PriceTo = tickSizeParams.PriceTo;
    }

    public Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Price64 TickSize { get; }
    public Price32 PriceFrom { get; }
    public Price32 PriceTo { get; }

    public static TickSizeMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 24) // Check for the minimum length of the message
            throw new ArgumentException("Invalid data format for TickSizeMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var orderBookId = reader.ReadNumeric32();
            var tickSize = reader.ReadPrice64();
            var priceFrom = reader.ReadPrice32();
            var priceTo = reader.ReadPrice32();

            //TODO: logic to get number of decimals for prices from order book id
            //Mock: number of decimals = 2
            var priceDecimals = 2;

            tickSize.NumberOfDecimals = priceDecimals;
            priceFrom.NumberOfDecimals = priceDecimals;
            priceTo.NumberOfDecimals = priceDecimals;

            var tickSizeParams = new TickSizeParams
            {
                Nanos = nanos,
                OrderBookId = orderBookId,
                TickSize = tickSize,
                PriceFrom = priceFrom,
                PriceTo = priceTo
            };

            return new TickSizeMessage(tickSizeParams);
        }
    }

    public string ToStringUnitTest()
    {
        return $"TickSizeMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookId: {OrderBookId},\n"
               + $"TickSize: {TickSize},\n"
               + $"PriceFrom: {PriceFrom},\n"
               + $"PriceTo: {PriceTo}";
    }
}