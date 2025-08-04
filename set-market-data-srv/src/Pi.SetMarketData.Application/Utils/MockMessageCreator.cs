using System.Text;
using Pi.SetMarketData.Application.Constants;

namespace Pi.SetMarketData.Application.Utils;

public static class MockMessageCreator
{
    public static byte[] CreateOrderBookDirectoryMessage()
    {
        var messageBuilder = new List<byte>
        {
            // Start with the message type for Order Book Directory, assumed to be 'R'
            (byte)'R'
        };

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 123456789, 4); // Nanos
        AddNumeric(ref messageBuilder, 98765, 4); // OrderBookID

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "ABCD", 32); // Symbol
        AddAlpha(ref messageBuilder, "ABCD Corporation", 32); // LongName
        AddAlpha(ref messageBuilder, "US1234567890", 12); // ISIN
        AddAlpha(ref messageBuilder, "ETF", 3); // FinancialProduct
        AddAlpha(ref messageBuilder, "USD", 3); // TradingCurrency

        // Add Numeric fields for price decimals
        AddNumeric(ref messageBuilder, 2, 2); // DecimalsInPrice
        AddNumeric(ref messageBuilder, 2, 2); // DecimalsInNominalValue

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 100, 4); // RoundLotSize
        AddNumeric(ref messageBuilder, 1000000, 8); // NominalValue
        AddNumeric(ref messageBuilder, 1, 1); // NumberOfLegs

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "LHHOT1", 6); // UnderlyingName

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 1234, 4); // Underlying
        AddNumeric(ref messageBuilder, 4321, 4); // UnderlyingOrderBookID

        // Add Price field
        AddPrice(ref messageBuilder, 10050, 4); // StrikePrice

        // Add Date field
        AddDate(ref messageBuilder, "20201231"); // ExpirationDate

        // Add Numeric fields for price decimals
        AddNumeric(ref messageBuilder, 2, 2); // DecimalsInStrikePrice
        AddNumeric(ref messageBuilder, 1, 1); // OptionType
        AddNumeric(ref messageBuilder, 1, 1); // ExchangeCode
        AddNumeric(ref messageBuilder, 1, 1); // MarketCode

        // Add Price fields
        AddPrice(ref messageBuilder, 12000, 8); // PriceQuotationFactor

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "", 32); // CorporateActionCode
        AddAlpha(ref messageBuilder, "", 32); // NotificationSign
        AddAlpha(ref messageBuilder, "", 32); // OtherSign
        AddAlpha(ref messageBuilder, "A", 1); // AllowNvdr
        AddAlpha(ref messageBuilder, "Y", 1); // AllowShortSell
        AddAlpha(ref messageBuilder, "N", 1); // AllowShortSellOnNvdr
        AddAlpha(ref messageBuilder, "Y", 1); // AllowTtf

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 1000, 8); // ParValue

        // Add Date and Time fields
        AddDate(ref messageBuilder, "20200101"); // FirstTradingDate
        AddTime(ref messageBuilder, "090000"); // FirstTradingTime
        AddDate(ref messageBuilder, "20201231"); // LastTradingDate
        AddTime(ref messageBuilder, "160000"); // LastTradingTime

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "MS", 4); // MarketSegment
        AddAlpha(ref messageBuilder, "PD", 1); // PhysicalDelivery

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 200, 4); // ContractSize

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "SC", 4); // SectorCode
        AddAlpha(ref messageBuilder, "Origin", 32); // OriginatesFrom
        AddAlpha(ref messageBuilder, "A", 1); // Status

        // Add Numeric fields for modifiers and other price decimals
        AddNumeric(ref messageBuilder, 0, 2); // Modifier

        AddDate(ref messageBuilder, "20200101"); // NotationDate

        AddNumeric(ref messageBuilder, 2, 2); // DecimalsInContractSizePQF

        return messageBuilder.ToArray();
    }

    public static byte[] CreateExchangeDirectoryMessage()
    {
        var messageBuilder = new List<byte> { (byte)'e' };

        AddNumeric(ref messageBuilder, 123456789, 4); // Nanos
        AddNumeric(ref messageBuilder, 35, 1); // Exchange Code
        AddAlpha(ref messageBuilder, "SET Exchange", 32); // Exchange Name

        return messageBuilder.ToArray();
    }

    public static byte[] CreateMarketDirectoryMessage()
    {
        var messageBuilder = new List<byte> { (byte)'m' };

        AddNumeric(ref messageBuilder, 987654321, 4); // Nanos: 4 bytes
        AddNumeric(ref messageBuilder, 1, 1); // Market Code: 1 byte
        AddAlpha(ref messageBuilder, "Equity Main Market", 32); // Market Name: 32 bytes, padded or trimmed to fit
        AddAlpha(ref messageBuilder, "EQSM", 5); // Market Description: 5 bytes, padded or trimmed to fit

        return messageBuilder.ToArray();
    }

    public static byte[] CreateCombinationOrderBookDirectoryMessage()
    {
        var messageBuilder = new List<byte>
        {
            (byte)'M' // Msg Type for Combination Order Book Directory Message
        };

        AddNumeric(ref messageBuilder, 987654321, 4); // Nanos: 4 bytes
        AddNumeric(ref messageBuilder, 1001, 4); // Combination Order book ID: 4 bytes
        AddNumeric(ref messageBuilder, 2002, 4); // Leg Order book ID: 4 bytes
        AddAlpha(ref messageBuilder, "B", 1); // Leg Side: 1 byte
        AddNumeric(ref messageBuilder, 1, 4); // Leg Ratio: 4 bytes

        return messageBuilder.ToArray();
    }

    public static byte[] CreateTickSizeMessage()
    {
        var messageBuilder = new List<byte> { (byte)'L' };

        // Nanos: 4 bytes
        AddNumeric(ref messageBuilder, 987654321, 4);

        // Order book ID: 4 bytes
        AddNumeric(ref messageBuilder, 1001, 4);

        // Tick Size: 8 bytes
        AddPrice(ref messageBuilder, 1, 8);

        // Price From: 4 bytes
        AddPrice(ref messageBuilder, 5000, 4);

        // Price To: 4 bytes
        AddPrice(ref messageBuilder, 10000, 4);

        return messageBuilder.ToArray();
    }

    public static byte[] CreatePriceLimitMessage()
    {
        var messageBuilder = new List<byte> { (byte)'k' };

        // Nanos: 4 bytes
        AddNumeric(ref messageBuilder, 987654321, 4);

        // Order book ID: 4 bytes
        AddNumeric(ref messageBuilder, 1001, 4);

        // Upper Limit: 4 bytes
        AddPrice(ref messageBuilder, 10000, 4);

        // Lower Limit To: 4 bytes
        AddPrice(ref messageBuilder, 1000, 4);

        return messageBuilder.ToArray();
    }

    public static byte[] CreateSystemEventMessage()
    {
        var messageBuilder = new List<byte> { (byte)'S' };

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 123456789, 4); // Nanos

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "O", 1); // EventCode

        return [.. messageBuilder];
    }

    public static byte[] CreateOrderBookStateMessage()
    {
        var messageBuilder = new List<byte> { (byte)'O' };

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 123456789, 4); // Nanos

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 12, 1); // OrderBookId

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "PRE-CLOSE_E", 20); // StateName

        return [.. messageBuilder];
    }

    public static byte[] CreateHaltInformationMessage()
    {
        var messageBuilder = new List<byte> { (byte)ItchMessageType.l };

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 123456789, 4); // Nanos

        // Add Numeric fields
        AddNumeric(ref messageBuilder, 12, 1); // OrderBookId

        // Add Alpha fields
        AddAlpha(ref messageBuilder, "SUSPEND_E", 20); // StateName

        return [.. messageBuilder];
    }

    // Method implementations for adding different types of data to the message
    public static void AddNumeric(ref List<byte> messageBuilder, long value, int size)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        messageBuilder.AddRange(bytes[^size..]);
    }

    public static void AddNumeric(ref List<byte> messageBuilder, ulong value, int size)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        messageBuilder.AddRange(bytes[^size..]);
    }

    public static void AddAlpha(ref List<byte> messageBuilder, string value, int size)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value.PadRight(size));
        messageBuilder.AddRange(bytes.Take(size));
    }

    public static void AddPrice(ref List<byte> messageBuilder, int value, int size)
    {
        AddNumeric(ref messageBuilder, value, size);
    }

    public static void AddPrice(ref List<byte> messageBuilder, long value, int size)
    {
        AddNumeric(ref messageBuilder, value, size);
    }

    public static void AddDate(ref List<byte> messageBuilder, string date)
    {
        // Assuming the date is in the format "YYYYMMDD"
        if (
            !DateTime.TryParseExact(
                date,
                "yyyyMMdd",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate
            )
        )
            throw new ArgumentException("Invalid date format.", nameof(date));

        int dateInt = (parsedDate.Year * 10000) + (parsedDate.Month * 100) + parsedDate.Day;
        AddNumeric(ref messageBuilder, dateInt, 4);
    }

    public static void AddTime(ref List<byte> messageBuilder, string time)
    {
        // Assuming the time is in the format "HHMMSS"
        if (
            !DateTime.TryParseExact(
                time,
                "HHmmss",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedTime
            )
        )
            throw new ArgumentException("Invalid time format.", nameof(time));

        int timeInt = (parsedTime.Hour * 10000) + (parsedTime.Minute * 100) + parsedTime.Second;
        AddNumeric(ref messageBuilder, timeInt, 4);
    }

    public static void AddTimestamp(ref List<byte> messageBuilder, string timeStamp)
    {
        if (
            !DateTime.TryParseExact(
                timeStamp,
                "yyyy-MM-dd HH:mm:ss",
                null,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedTimeStamp
            )
        )
            throw new ArgumentException("Invalid timestamp format.", nameof(timeStamp));

        TimeSpan durationSinceEpoch =
            parsedTimeStamp - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long nanosecondsSinceEpoch = (long)(durationSinceEpoch.TotalSeconds * 1_000_000_000); // Convert from seconds to nanoseconds

        byte[] nanosecondsBytes = BitConverter.GetBytes(nanosecondsSinceEpoch);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(nanosecondsBytes);

        if (nanosecondsBytes.Length != 8)
            throw new InvalidOperationException("Unexpected byte array length for timestamp.");

        messageBuilder.AddRange(nanosecondsBytes);
    }
}
