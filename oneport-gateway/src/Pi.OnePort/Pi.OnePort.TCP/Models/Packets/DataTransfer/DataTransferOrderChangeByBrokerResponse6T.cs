using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangeByBrokerResponse6T : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.OrderChangeByBrokerResponse6T;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string Account { get; init; }
    public required string PortClient { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderType OrderType { get; init; }
    public required decimal Price { get; init; }
    public required ConPrice ConPrice { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PublishVolume { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new DataTransferOrderChangeByBrokerResponse6T
        {
            RefOrderId = helper.Next(FieldLength.RefOrderId).Trim(),
            FisOrderId = helper.Next(FieldLength.FisOrderId).Trim(),
            EnterId = helper.Next(FieldLength.EnterId).Trim(),
            Account = helper.Next(FieldLength.Account).Trim(),
            PortClient = helper.Next(FieldLength.PortClient).Trim(),
            Ttf = EnumExtensions.ParseFromSerializedValue<Ttf>(helper.Next(FieldLength.Ttf)),
            OrderType = EnumExtensions.ParseFromSerializedValue<OrderType>(helper.Next(FieldLength.OrderType)),
            Price = decimal.Parse(helper.Next(FieldLength.Price)),
            ConPrice = EnumExtensions.ParseFromSerializedValue<ConPrice>(helper.Next(FieldLength.ConPrice)),
            Volume = decimal.Parse(helper.Next(FieldLength.Volume)),
            PublishVolume = decimal.Parse(helper.Next(FieldLength.PublishVolume)),
        };
    }
}
