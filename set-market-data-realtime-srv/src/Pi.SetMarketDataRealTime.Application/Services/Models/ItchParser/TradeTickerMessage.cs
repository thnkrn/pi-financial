using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class TradeTickerParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderbookId { get; init; }
    public required Numeric64 DealId { get; init; }
    public required Numeric8 DealSource { get; init; }
    public required Price32 Price { get; init; }
    public required Numeric64 Quantity { get; init; }
    public required Timestamp DealDateTime { get; init; }
    public required Numeric8 Action { get; init; }
    public required Alpha Aggressor { get; init; }
    public required Numeric16 TradeReportCode { get; init; }
}

public class TradeTickerMessage : ItchMessage
{
    private const int MinimumByteLength = 41;
    private const char MessageTypeChar = 'i';

    public TradeTickerMessage(TradeTickerParams tradeTickerParams)
    {
        MsgType = MessageTypeChar;
        Nanos = tradeTickerParams.Nanos;
        OrderbookId = tradeTickerParams.OrderbookId;
        DealId = tradeTickerParams.DealId;
        DealSource = tradeTickerParams.DealSource;
        Price = tradeTickerParams.Price;
        Quantity = tradeTickerParams.Quantity;
        DealDateTime = tradeTickerParams.DealDateTime;
        Action = tradeTickerParams.Action;
        Aggressor = tradeTickerParams.Aggressor;
        TradeReportCode = tradeTickerParams.TradeReportCode;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderbookId { get; }
    public Numeric64 DealId { get; }
    public Numeric8 DealSource { get; }
    public Price32 Price { get; }
    public Numeric64 Quantity { get; }
    public Timestamp DealDateTime { get; }
    public Numeric8 Action { get; }
    public Alpha Aggressor { get; }
    public Numeric16 TradeReportCode { get; }

    public string DealIdHex => DealId.Value.ToString("X16");

    public static TradeTickerMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < MinimumByteLength)
            throw new ArgumentException(
                $"Invalid data format for TradeTickerMessage. Expected at least {MinimumByteLength} bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        try
        {
            return new TradeTickerMessage(new TradeTickerParams
            {
                Nanos = reader.ReadNumeric32(),
                OrderbookId = reader.ReadNumeric32(),
                DealId = reader.ReadNumeric64(),
                DealSource = reader.ReadNumeric8(),
                Price = reader.ReadPrice32(),
                Quantity = reader.ReadNumeric64(),
                DealDateTime = reader.ReadTimestamp(),
                Action = reader.ReadNumeric8(),
                Aggressor = reader.ReadAlpha(1),
                TradeReportCode = reader.ReadNumeric16()
            });
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error parsing TradeTickerMessage", nameof(bytes), ex);
        }
    }

    public string ToStringUnitTest()
    {
        return $"""
                TradeTickerMessage:
                Nanos: {Nanos},
                OrderbookId: {OrderbookId},
                DealId: {DealIdHex},
                DealSource: {DealSource},
                Price: {Price},
                Quantity: {Quantity},
                DealDateTime: {DealDateTime},
                Action: {Action},
                Aggressor: {Aggressor},
                TradeReportCode: {TradeReportCode}
                """;
    }
}