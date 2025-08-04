using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class MarketAnnouncementMessageParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 OrderBookId { get; init; }
    public required Numeric8 MarketCode { get; init; }
    public required Numeric8 ExchangeCode { get; init; }
    public required Numeric8 MessageInformationType { get; init; }
    public required Alpha MessageSource { get; init; }
    public required Numeric8 MessagePriority { get; init; }
    public required Alpha MessageHeader { get; init; }
    public required Numeric8 ItemsURL { get; init; }
    public required Alpha DocumentURL { get; init; }
    public required Numeric8 NumberOfMessageItems { get; init; }
    public required Numeric8 NumberOfMessageItemsInArray { get; init; }
    public required IReadOnlyList<MessageItem> ArrayOfMessageItems { get; init; }
}

public class MarketAnnouncementMessage : ItchMessage
{
    public MarketAnnouncementMessage(MarketAnnouncementMessageParams messageParams)
    {
        MsgType = ItchMessageType.N;
        Nanos = messageParams.Nanos;
        OrderBookId = messageParams.OrderBookId;
        MarketCode = messageParams.MarketCode;
        ExchangeCode = messageParams.ExchangeCode;
        MessageInformationType = messageParams.MessageInformationType;
        MessageSource = messageParams.MessageSource;
        MessagePriority = messageParams.MessagePriority;
        MessageHeader = messageParams.MessageHeader;
        ItemsURL = messageParams.ItemsURL;
        DocumentURL = messageParams.DocumentURL;
        NumberOfMessageItems = messageParams.NumberOfMessageItems;
        NumberOfMessageItemsInArray = messageParams.NumberOfMessageItemsInArray;
        ArrayOfMessageItems = messageParams.ArrayOfMessageItems;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 OrderBookId { get; }
    public Numeric8 MarketCode { get; }
    public Numeric8 ExchangeCode { get; }
    public Numeric8 MessageInformationType { get; }
    public Alpha MessageSource { get; }
    public Numeric8 MessagePriority { get; }
    public Alpha MessageHeader { get; }
    public Numeric8 ItemsURL { get; }
    public Alpha DocumentURL { get; }
    public Numeric8 NumberOfMessageItems { get; }
    public Numeric8 NumberOfMessageItemsInArray { get; }
    public IReadOnlyList<MessageItem> ArrayOfMessageItems { get; }

    public static MarketAnnouncementMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 510)
            throw new ArgumentException(
                "Invalid data format for MarketAnnouncementMessage. Expected at least 510 bytes.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var messageItems = new List<MessageItem>();

        var messageParams = new MarketAnnouncementMessageParams
        {
            Nanos = reader.ReadNumeric32(),
            OrderBookId = reader.ReadNumeric32(),
            MarketCode = reader.ReadNumeric8(),
            ExchangeCode = reader.ReadNumeric8(),
            MessageInformationType = reader.ReadNumeric8(),
            MessageSource = reader.ReadAlpha(80),
            MessagePriority = reader.ReadNumeric8(),
            MessageHeader = reader.ReadAlpha(80),
            ItemsURL = reader.ReadNumeric8(),
            DocumentURL = reader.ReadAlpha(255),
            NumberOfMessageItems = reader.ReadNumeric8(),
            NumberOfMessageItemsInArray = reader.ReadNumeric8(),
            ArrayOfMessageItems = messageItems
        };

        while (!reader.EndOfStream) messageItems.Add(MessageItem.Parse(reader));

        return new MarketAnnouncementMessage(messageParams);
    }

    public string ToStringUnitTest()
    {
        return $"""
                MarketAnnouncementMessage:
                Nanos: {Nanos},
                OrderBookId: {OrderBookId},
                MarketCode: {MarketCode},
                ExchangeCode: {ExchangeCode},
                MessageInformationType: {MessageInformationType},
                MessageSource: {MessageSource},
                MessagePriority: {MessagePriority},
                MessageHeader: {MessageHeader},
                ItemsURL: {ItemsURL},
                DocumentURL: {DocumentURL},
                NumberOfMessageItems: {NumberOfMessageItems},
                NumberOfMessageItemsInArray: {NumberOfMessageItemsInArray},
                ArrayOfMessageItems: {string.Join(", ", ArrayOfMessageItems)}
                """;
    }
}