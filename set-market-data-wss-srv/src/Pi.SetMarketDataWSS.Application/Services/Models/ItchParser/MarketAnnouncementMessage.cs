using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class MarketAnnouncementMessageParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookId { get; set; }
    public Numeric8 MarketCode { get; set; }
    public Numeric8 ExchangeCode { get; set; }
    public Numeric8 MessageInformationType { get; set; }
    public required Alpha MessageSource { get; set; }
    public Numeric8 MessagePriority { get; set; }
    public required Alpha MessageHeader { get; set; }
    public Numeric8 ItemsURL { get; set; }
    public required Alpha DocumentURL { get; set; }
    public Numeric8 NumberOfMessageItems { get; set; }
    public Numeric8 NumberOfMessageItemsInArray { get; set; }
    public required List<MessageItem> ArrayOfMessageItems { get; set; }
}

public class MarketAnnouncementMessage : ItchMessage
{
    public MarketAnnouncementMessage(
        MarketAnnouncementMessageParams marketAnnouncementMessageParams
    )
    {
        MsgType = ItchMessageType.N;
        Nanos = marketAnnouncementMessageParams.Nanos;
        OrderBookId = marketAnnouncementMessageParams.OrderBookId;
        MarketCode = marketAnnouncementMessageParams.MarketCode;
        ExchangeCode = marketAnnouncementMessageParams.ExchangeCode;
        MessageInformationType = marketAnnouncementMessageParams.MessageInformationType;
        MessageSource = marketAnnouncementMessageParams.MessageSource;
        MessagePriority = marketAnnouncementMessageParams.MessagePriority;
        MessageHeader = marketAnnouncementMessageParams.MessageHeader;
        ItemsURL = marketAnnouncementMessageParams.ItemsURL;
        DocumentURL = marketAnnouncementMessageParams.DocumentURL;
        NumberOfMessageItems = marketAnnouncementMessageParams.NumberOfMessageItems;
        NumberOfMessageItemsInArray =
            marketAnnouncementMessageParams.NumberOfMessageItemsInArray;
        ArrayOfMessageItems = marketAnnouncementMessageParams.ArrayOfMessageItems;
    }

    public Numeric32 Nanos { get; }
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
    public List<MessageItem> ArrayOfMessageItems { get; }

    public static MarketAnnouncementMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 510) // Expecting exactly 510 bytes for the MarketAnnouncementMessage
            throw new ArgumentException("Invalid data format for MarketAnnouncementMessage.");
        Memory<byte> bytesMemory = new(bytes);

        ItchMessageByteReader reader = new(bytesMemory);
        var nanos = reader.ReadNumeric32();
        var orderBookId = reader.ReadNumeric32();
        var marketCode = reader.ReadNumeric8();
        var exchangeCode = reader.ReadNumeric8();
        var messageInformationType = reader.ReadNumeric8();
        var messageSource = reader.ReadAlpha(80);
        var messagePriority = reader.ReadNumeric8();
        var messageHeader = reader.ReadAlpha(80);
        var itemsURL = reader.ReadNumeric8();
        var documentURL = reader.ReadAlpha(255);
        var numberOfMessageItems = reader.ReadNumeric8();
        var numberOfMessageItemsInArray = reader.ReadNumeric8();
        List<MessageItem> arrayOfMessageItems = [];

        while (!reader.EndOfStream)
        {
            var messageItem = MessageItem.Parse(reader);
            arrayOfMessageItems.Add(messageItem);
        }

        MarketAnnouncementMessageParams marketAnnouncementMessageParams =
            new()
            {
                Nanos = nanos,
                OrderBookId = orderBookId,
                MarketCode = marketCode,
                ExchangeCode = exchangeCode,
                MessageInformationType = messageInformationType,
                MessageSource = messageSource,
                MessagePriority = messagePriority,
                MessageHeader = messageHeader,
                ItemsURL = itemsURL,
                DocumentURL = documentURL,
                NumberOfMessageItems = numberOfMessageItems,
                NumberOfMessageItemsInArray = numberOfMessageItemsInArray,
                ArrayOfMessageItems = arrayOfMessageItems
            };

        return new MarketAnnouncementMessage(marketAnnouncementMessageParams);
    }

    public string ToStringUnitTest()
    {
        return $"MarketAnnouncementMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"OrderBookId: {OrderBookId},\n"
               + $"MarketCode: {MarketCode},\n"
               + $"ExchangeCode: {ExchangeCode},\n"
               + $"MessageInformationType: {MessageInformationType},\n"
               + $"MessageSource: {MessageSource},\n"
               + $"MessagePriority: {MessagePriority},\n"
               + $"MessageHeader: {MessageHeader},\n"
               + $"ItemsURL: {ItemsURL},\n"
               + $"DocumentURL: {DocumentURL},\n"
               + $"NumberOfMessageItems: {NumberOfMessageItems},\n"
               + $"NumberOfMessageItemsInArray: {NumberOfMessageItemsInArray},\n"
               + $"ArrayOfMessageItems: {ArrayOfMessageItems}\n";
    }
}