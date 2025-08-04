using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketDataTransfer(IDataTransferPacket DataTransferPacketContent) : IPacketDataInBound, IPacketDataOutBound
{
    public PacketType PacketType => PacketType.DataTransfer;

    public string Serialize()
    {
        if (DataTransferPacketContent is not IDataTransferPacketInBound inBound)
        {
            throw new InvalidOperationException();
        }

        return inBound.Serialize();
    }
    public static IPacketData Deserialize(string serializedPacketData)
    {
        var messageTypeStr = serializedPacketData[..FieldLength.MessageType].Trim();
        var messageType = EnumExtensions.TryGetEnumFromAttribute<MessageType, SerializedValue>(messageTypeStr, MessageType.Unknown);
        var content = serializedPacketData[FieldLength.MessageType..];

        return new PacketDataTransfer(
            messageType switch
            {
                MessageType.OrderAcknowledgementResponse7K => DataTransferOrderAcknowledgementResponse7K.Deserialize(content),
                MessageType.ExecutionReportResponse7E => DataTransferExecutionReportResponse7E.Deserialize(content),
                MessageType.NewOrderResponse6A => DataTransferNewOrderResponse6A.Deserialize(content),
                MessageType.OrderChangeResponse7N => DataTransferOrderChangeResponse7N.Deserialize(content),
                MessageType.OrderChangeByBrokerResponse6T => DataTransferOrderChangeByBrokerResponse6T.Deserialize(content),
                MessageType.ConfirmCancelDealResponse3D => DataTransferConfirmCancelDealResponse3D.Deserialize(content),
                MessageType.ExecutionReportResponse7X => DataTransferExecutionReportResponse7X.Deserialize(content),
                MessageType.OrderChangeConfirmResponse7V => DataTransferOrderChangeConfirmResponse7V.Deserialize(content),
                MessageType.OrderChangePutThroughFromBrokerResponse6W => DataTransferOrderChangePutThroughFromBrokerResponse6W.Deserialize(content),
                MessageType.Unknown => DataTransferUnknownPacket.Deserialize(content), // For debug
                _ => throw new ArgumentOutOfRangeException($"Invalid type, packet data: '{serializedPacketData}'")
            }
        );
    }
}
