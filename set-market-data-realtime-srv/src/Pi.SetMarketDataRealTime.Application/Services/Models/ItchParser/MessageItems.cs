using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class MessageItemParams
{
    public required Alpha AdvertisementId { get; init; }
    public required Alpha Action { get; init; }
    public required Alpha Side { get; init; }
    public required Alpha Quantity { get; init; }
    public required Alpha Price { get; init; }
    public required Alpha ContactName { get; init; }
    public required Alpha ContactInfo { get; init; }
}

public class MessageItem
{
    public MessageItem(MessageItemParams messageItemParams)
    {
        AdvertisementId = messageItemParams.AdvertisementId;
        Action = messageItemParams.Action;
        Side = messageItemParams.Side;
        Quantity = messageItemParams.Quantity;
        Price = messageItemParams.Price;
        ContactName = messageItemParams.ContactName;
        ContactInfo = messageItemParams.ContactInfo;
    }

    public Alpha AdvertisementId { get; }
    public Alpha Action { get; }
    public Alpha Side { get; }
    public Alpha Quantity { get; }
    public Alpha Price { get; }
    public Alpha ContactName { get; }
    public Alpha ContactInfo { get; }

    public static MessageItem Parse(ItchMessageByteReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        return new MessageItem(new MessageItemParams
        {
            AdvertisementId = reader.ReadAlpha(10),
            Action = reader.ReadAlpha(1),
            Side = reader.ReadAlpha(1),
            Quantity = reader.ReadAlpha(20),
            Price = reader.ReadAlpha(10),
            ContactName = reader.ReadAlpha(10),
            ContactInfo = reader.ReadAlpha(28)
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                MessageItems:
                AdvertisementId: {AdvertisementId},
                Action: {Action},
                Side: {Side},
                Quantity: {Quantity},
                Price: {Price},
                ContactName: {ContactName},
                ContactInfo: {ContactInfo}
                """;
    }
}