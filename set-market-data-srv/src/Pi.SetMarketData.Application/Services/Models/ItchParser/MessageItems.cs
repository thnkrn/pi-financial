using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class MessageItemParams
    {
        public required Alpha AdvertisementId { get; set; }
        public required Alpha Action { get; set; }
        public required Alpha Side { get; set; }
        public required Alpha Quantity { get; set; }
        public required Alpha Price { get; set; }
        public required Alpha ContactName { get; set; }
        public required Alpha ContactInfo { get; set; }
    }

    public class MessageItem(MessageItemParams messageItemsParams)
    {
        public Alpha AdvertisementId { get; private set; } = messageItemsParams.AdvertisementId;
        public Alpha Action { get; private set; } = messageItemsParams.Action;
        public Alpha Side { get; private set; } = messageItemsParams.Side;
        public Alpha Quantity { get; private set; } = messageItemsParams.Quantity;
        public Alpha Price { get; private set; } = messageItemsParams.Price;
        public Alpha ContactName { get; private set; } = messageItemsParams.ContactName;
        public Alpha ContactInfo { get; private set; } = messageItemsParams.ContactInfo;

        public static MessageItem Parse(ItchMessageByteReader reader)
        {
            var advertisementId = reader.ReadAlpha(10);
            var action = reader.ReadAlpha(1);
            var side = reader.ReadAlpha(1);
            var quantity = reader.ReadAlpha(20);
            var price = reader.ReadAlpha(10);
            var contactName = reader.ReadAlpha(10);
            var contactInfo = reader.ReadAlpha(28);

            var messageItemsParams = new MessageItemParams
            {
                AdvertisementId = advertisementId,
                Action = action,
                Side = side,
                Quantity = quantity,
                Price = price,
                ContactName = contactName,
                ContactInfo = contactInfo,
            };

            return new MessageItem(messageItemsParams);
        }

        public override string ToString()
        {
            return $"MessageItems:\n"
                + $"AdvertisementId: {AdvertisementId},\n"
                + $"Action: {Action},\n"
                + $"Side: {Side},\n"
                + $"Quantity: {Quantity},\n"
                + $"Price: {Price},\n"
                + $"ContactName: {ContactName},\n"
                + $"ContactInfo: {ContactInfo},\n";
        }
    }
}
