using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class IndexPriceParams
{
    // Nanoseconds portion of the timestamp.
    public Numeric32 Nanos { get; set; }

    // Orderbook ID.
    public Numeric32 OrderbookId { get; set; }

    // Index last value of current day.
    public Price64 Value { get; set; }

    // Index high value of current day.
    public Price64 HighValue { get; set; }

    // Index low value of current day.
    public Price64 LowValue { get; set; }

    // Index open value of current day.
    public Price64 OpenValue { get; set; }

    // Index traded volume of current day.
    public Numeric64 TradedVolume { get; set; }

    // Index traded value of current day.
    public Price64 TradedValue { get; set; }

    // Index change compared to previous close.
    public Price64 Change { get; set; }

    // Index change percent compared to previous close (2 decimal digits).
    public Price32 ChangePercent { get; set; }

    // Index close value of previous day.
    public Price64 PreviousClose { get; set; }

    // Index Close value of current day.
    public Price64 Close { get; set; }

    // The time the Index value.
    public Timestamp Timestamp { get; set; }
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

    public Numeric32 Nanos { get; }
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
        if (bytes == null || bytes.Length < 92) // Ensure byte array is of expected length
            throw new ArgumentException("Invalid data format for IndexPriceMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
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
    }

    public string ToStringUnitTest()
    {
        return $"IndexPriceMessage:\n"
               + $"MsgType: {MsgType}, Nanos: {Nanos}, OrderbookId: {OrderbookId}, Value: {Value}, "
               + $"HighValue: {HighValue}, LowValue: {LowValue}, OpenValue: {OpenValue}, "
               + $"TradedVolume: {TradedVolume}, TradedValue: {TradedValue}, Change: {Change}, "
               + $"ChangePercent: {ChangePercent}, PreviousClose: {PreviousClose}, Close: {Close}, "
               + $"Timestamp: {Timestamp}";
    }
}