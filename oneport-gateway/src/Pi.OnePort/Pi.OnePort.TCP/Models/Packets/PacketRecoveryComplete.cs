using Pi.OnePort.TCP.Enums;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketRecoveryComplete : IPacketDataInBound, IPacketDataOutBound
{
    public PacketType PacketType => PacketType.RecoveryComplete;
    public string Serialize() => "";

    public static IPacketData Deserialize(string serializedPacketData) => new PacketRecoveryComplete();
}
