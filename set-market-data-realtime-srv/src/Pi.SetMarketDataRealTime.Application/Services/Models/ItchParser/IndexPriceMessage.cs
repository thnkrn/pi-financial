using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class IndexPriceParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderbookId { get; init; }
    public required Price64 Value { get; init; }
    public required Price64 HighValue { get; init; }
    public required Price64 LowValue { get; init; }
    public required Price64 OpenValue { get; init; }
    public required Numeric64 TradedVolume { get; init; }
    public required Price64 TradedValue { get; init; }
    public required Price64 Change { get; init; }
    public required Price32 ChangePercent { get; init; }
    public required Price64 PreviousClose { get; init; }
    public required Price64 Close { get; init; }
    public required Timestamp Timestamp { get; init; }
}

public class IndexPriceMessage : ItchMessage
{
    public IndexPriceMessage(IndexPriceParams indexPriceParams)
    {
        MsgType = 'J'; // 'J' – Index Price Message
        Nanos = indexPriceParams.Nanos;
        OrderbookId = indexPriceParams.OrderbookId;
        Value = indexPriceParams.Value;
        HighValue = indexPriceParams.HighValue;
        LowValue = indexPriceParams.LowValue;
        OpenValue = indexPriceParams.OpenValue;
        TradedVolume = indexPriceParams.TradedVolume;
        TradedValue = indexPriceParams.TradedValue;
        Change = indexPriceParams.Change;
        ChangePercent = indexPriceParams.ChangePercent;
        PreviousClose = indexPriceParams.PreviousClose;
        Close = indexPriceParams.Close;
        Timestamp = indexPriceParams.Timestamp;
    }

    // Match the base class accessor for Nanos
    public override Numeric32 Nanos { get; }

    public Numeric32 OrderbookId { get; }
    public Price64 Value { get; }
    public Price64 HighValue { get; }
    public Price64 LowValue { get; }
    public Price64 OpenValue { get; }
    public Numeric64 TradedVolume { get; }
    public Price64 TradedValue { get; }
    public Price64 Change { get; }
    public Price32 ChangePercent { get; }
    public Price64 PreviousClose { get; }
    public Price64 Close { get; }
    public Timestamp Timestamp { get; }

    public static IndexPriceMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 92)
            throw new ArgumentException("Invalid data format for IndexPriceMessage. Expected at least 92 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        return new IndexPriceMessage(
            new IndexPriceParams
            {
                Nanos = reader.ReadNumeric32(),
                OrderbookId = reader.ReadNumeric32(),
                Value = reader.ReadPrice64(),
                HighValue = reader.ReadPrice64(),
                LowValue = reader.ReadPrice64(),
                OpenValue = reader.ReadPrice64(),
                TradedVolume = reader.ReadNumeric64(),
                TradedValue = reader.ReadPrice64(),
                Change = reader.ReadPrice64(),
                ChangePercent = reader.ReadPrice32(),
                PreviousClose = reader.ReadPrice64(),
                Close = reader.ReadPrice64(),
                Timestamp = reader.ReadTimestamp()
            }
        );
    }

    public string ToStringUnitTest()
    {
        return $"""
                IndexPriceMessage:
                MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, Value: {Value},
                HighValue: {HighValue}, LowValue: {LowValue}, OpenValue: {OpenValue},
                TradedVolume: {TradedVolume}, TradedValue: {TradedValue}, Change: {Change},
                ChangePercent: {ChangePercent}, PreviousClose: {PreviousClose}, Close: {Close},
                Timestamp: {Timestamp}
                """;
    }
}