using Pi.OnePort.TCP.Enums.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferUnknownPacket(string Content) : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.Unknown;

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        return new DataTransferUnknownPacket(serializedPacketData);
    }
}
