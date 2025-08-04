using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangePutThroughFromBrokerResponse6W : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.OrderChangePutThroughFromBrokerResponse6W;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string Account { get; init; }
    public required Ttf Ttf { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new DataTransferOrderChangePutThroughFromBrokerResponse6W
        {
            RefOrderId = helper.Next(FieldLength.RefOrderId).Trim(),
            FisOrderId = helper.Next(FieldLength.FisOrderId).Trim(),
            EnterId = helper.Next(FieldLength.EnterId).Trim(),
            Account = helper.Next(FieldLength.Account).Trim(),
            Ttf = EnumExtensions.ParseFromSerializedValue<Ttf>(helper.Next(FieldLength.Ttf)),
        };
    }
}
