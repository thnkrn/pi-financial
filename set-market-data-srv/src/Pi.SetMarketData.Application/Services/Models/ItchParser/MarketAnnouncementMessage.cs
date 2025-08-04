using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
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
        public Numeric32 Nanos { get; private set; }
        public Numeric32 OrderBookId { get; private set; }
        public Numeric8 MarketCode { get; private set; }
        public Numeric8 ExchangeCode { get; private set; }
        public Numeric8 MessageInformationType { get; private set; }
        public Alpha MessageSource { get; private set; }

        public Numeric8 MessagePriority { get; private set; }
        public Alpha MessageHeader { get; private set; }
        public Numeric8 ItemsURL { get; private set; }
        public Alpha DocumentURL { get; private set; }
        public Numeric8 NumberOfMessageItems { get; private set; }
        public Numeric8 NumberOfMessageItemsInArray { get; private set; }
        public List<MessageItem> ArrayOfMessageItems { get; private set; }

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

        public static MarketAnnouncementMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 510) // Expecting exactly 510 bytes for the MarketAnnouncementMessage
            {
                throw new ArgumentException("Invalid data format for MarketAnnouncementMessage.");
            }
            Memory<byte> bytesMemory = new(bytes);

            ItchMessageByteReader reader = new(bytesMemory);
            Numeric32 nanos = reader.ReadNumeric32();
            Numeric32 orderBookId = reader.ReadNumeric32();
            Numeric8 marketCode = reader.ReadNumeric8();
            Numeric8 exchangeCode = reader.ReadNumeric8();
            Numeric8 messageInformationType = reader.ReadNumeric8();
            Alpha messageSource = reader.ReadAlpha(80);
            Numeric8 messagePriority = reader.ReadNumeric8();
            Alpha messageHeader = reader.ReadAlpha(80);
            Numeric8 itemsURL = reader.ReadNumeric8();
            Alpha documentURL = reader.ReadAlpha(255);
            Numeric8 numberOfMessageItems = reader.ReadNumeric8();
            Numeric8 numberOfMessageItemsInArray = reader.ReadNumeric8();
            List<MessageItem> arrayOfMessageItems = [];

            while (!reader.EndOfStream)
            {
                MessageItem messageItem = MessageItem.Parse(reader);
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
                    ArrayOfMessageItems = arrayOfMessageItems,
                };

            return new MarketAnnouncementMessage(marketAnnouncementMessageParams);
        }

        public override string ToString()
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
}
