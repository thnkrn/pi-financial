using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class PriceLimitParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderbookId { get; set; }
    public Price32 UpperLimit { get; set; }
    public Price32 LowerLimit { get; set; }
}

public class PriceLimitMessage : ItchMessage
{
    public PriceLimitMessage(PriceLimitParams priceLimitParams)
    {
        MsgType = ItchMessageType.k; // Msg Type for Price Limit Message
        Nanos = priceLimitParams.Nanos;
        OrderbookId = priceLimitParams.OrderbookId;
        UpperLimit = priceLimitParams.UpperLimit;
        LowerLimit = priceLimitParams.LowerLimit;
    }

    public Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Price32 UpperLimit { get; }
    public Price32 LowerLimit { get; }

    public static PriceLimitMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 16) // Check for the minimum length of the message
            throw new ArgumentException("Invalid data format for PriceLimitMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var orderbookId = reader.ReadNumeric32();
            var upperLimit = reader.ReadPrice32();
            var lowerLimit = reader.ReadPrice32();

            //TODO: get priceDecimals from order book id
            var priceDecimals = 2;

            upperLimit.NumberOfDecimals = priceDecimals;
            lowerLimit.NumberOfDecimals = priceDecimals;

            upperLimit = AdjustedUpperLimit(upperLimit);
            lowerLimit = AdjustedLowerLimit(lowerLimit);

            var priceLimitParams = new PriceLimitParams
            {
                Nanos = nanos,
                OrderbookId = orderbookId,
                UpperLimit = upperLimit,
                LowerLimit = lowerLimit
            };

            return new PriceLimitMessage(priceLimitParams);
        }
    }

    public string ToStringUnitTest()
    {
        return $"PriceLimitMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderbookId: {OrderbookId},\n"
               + $"UpperLimit: {UpperLimit},\n"
               + $"LowerLimit: {LowerLimit}";
    }

    public static Price32 AdjustedUpperLimit(Price32 upperLimit)
    {
        if (upperLimit == -2147483648)
        {
            var wholeBaseValue = (int)(
                (int)(2147483648 / Math.Pow(10, upperLimit.NumberOfDecimals))
                * Math.Pow(10, upperLimit.NumberOfDecimals)
            );
            return new Price32(wholeBaseValue) { NumberOfDecimals = upperLimit.NumberOfDecimals };
        }

        return upperLimit;
    }

    public static Price32 AdjustedLowerLimit(Price32 lowerLimit)
    {
        if (lowerLimit == -2147483648) return new Price32(1) { NumberOfDecimals = 2 };

        return lowerLimit;
    }
}