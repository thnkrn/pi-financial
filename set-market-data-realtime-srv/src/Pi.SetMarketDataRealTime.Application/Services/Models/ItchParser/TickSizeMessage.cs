using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class TickSizeParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Price64 TickSize { get; init; }
    public required Price32 PriceFrom { get; init; }
    public required Price32 PriceTo { get; init; }
}

public class TickSizeMessage : ItchMessage
{
    private const int MinimumByteLength = 24;
    private const int DefaultPriceDecimals = 2;

    public TickSizeMessage(TickSizeParams tickSizeParams)
    {
        MsgType = ItchMessageType.L;
        Nanos = tickSizeParams.Nanos;
        OrderBookId = tickSizeParams.OrderBookId;
        TickSize = tickSizeParams.TickSize;
        PriceFrom = tickSizeParams.PriceFrom;
        PriceTo = tickSizeParams.PriceTo;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Price64 TickSize { get; }
    public Price32 PriceFrom { get; }
    public Price32 PriceTo { get; }

    public static TickSizeMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < MinimumByteLength)
            throw new ArgumentException(
                $"Invalid data format for TickSizeMessage. Expected at least {MinimumByteLength} bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var tickSize = reader.ReadPrice64();
        var priceFrom = reader.ReadPrice32();
        var priceTo = reader.ReadPrice32();

        var priceDecimals = GetPriceDecimals(orderBookId);

        tickSize.NumberOfDecimals = priceDecimals;
        priceFrom.NumberOfDecimals = priceDecimals;
        priceTo.NumberOfDecimals = priceDecimals;

        return new TickSizeMessage(new TickSizeParams
        {
            Nanos = nanos,
            OrderBookId = orderBookId,
            TickSize = tickSize,
            PriceFrom = priceFrom,
            PriceTo = priceTo
        });
    }

    private static int GetPriceDecimals(Numeric32 orderBookId)
    {
        // TODO: Implement logic to get price decimals based on orderBookId
        return DefaultPriceDecimals;
    }

    public string ToStringUnitTest()
    {
        return $"""
                TickSizeMessage:
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                TickSize: {TickSize},
                PriceFrom: {PriceFrom},
                PriceTo: {PriceTo}
                """;
    }
}