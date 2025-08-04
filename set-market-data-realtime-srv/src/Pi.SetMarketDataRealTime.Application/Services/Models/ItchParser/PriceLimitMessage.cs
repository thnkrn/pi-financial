using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class PriceLimitParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderbookId { get; init; }
    public required Price32 UpperLimit { get; init; }
    public required Price32 LowerLimit { get; init; }
}

public class PriceLimitMessage : ItchMessage
{
    private const int NoPriceIndicator = -2147483648;

    public PriceLimitMessage(PriceLimitParams priceLimitParams)
    {
        MsgType = ItchMessageType.k;
        Nanos = priceLimitParams.Nanos;
        OrderbookId = priceLimitParams.OrderbookId;
        UpperLimit = priceLimitParams.UpperLimit;
        LowerLimit = priceLimitParams.LowerLimit;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Price32 UpperLimit { get; }
    public Price32 LowerLimit { get; }

    public static PriceLimitMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 16)
            throw new ArgumentException("Invalid data format for PriceLimitMessage. Expected at least 16 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderbookId = reader.ReadNumeric32();
        var upperLimit = reader.ReadPrice32();
        var lowerLimit = reader.ReadPrice32();

        var priceDecimals = GetPriceDecimals(orderbookId);

        upperLimit.NumberOfDecimals = priceDecimals;
        lowerLimit.NumberOfDecimals = priceDecimals;

        return new PriceLimitMessage(new PriceLimitParams
        {
            Nanos = nanos,
            OrderbookId = orderbookId,
            UpperLimit = AdjustUpperLimit(upperLimit),
            LowerLimit = AdjustLowerLimit(lowerLimit)
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                PriceLimitMessage:
                Nanos: {Nanos},
                OrderbookId: {OrderbookId},
                UpperLimit: {UpperLimit},
                LowerLimit: {LowerLimit}
                """;
    }

    private static Price32 AdjustUpperLimit(Price32 upperLimit)
    {
        if (upperLimit.Value == NoPriceIndicator)
        {
            var wholeBaseValue = (int)(
                ((long)int.MaxValue + 1) / (long)Math.Pow(10, upperLimit.NumberOfDecimals)
            ) * (int)Math.Pow(10, upperLimit.NumberOfDecimals);
            return new Price32(wholeBaseValue) { NumberOfDecimals = upperLimit.NumberOfDecimals };
        }

        return upperLimit;
    }

    private static Price32 AdjustLowerLimit(Price32 lowerLimit)
    {
        if (lowerLimit.Value == NoPriceIndicator) return new Price32(1) { NumberOfDecimals = 2 };
        return lowerLimit;
    }

    private static int GetPriceDecimals(Numeric32 orderbookId)
    {
        // TODO: Implement logic to get price decimals based on orderbookId
        return 2; // Default value
    }
}