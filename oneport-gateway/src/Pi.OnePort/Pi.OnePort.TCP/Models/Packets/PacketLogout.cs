using Pi.OnePort.TCP.Enums;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketLogout : IPacketDataInBound, IPacketDataOutBound
{
    public PacketType PacketType => PacketType.Logout;
    public string Serialize() => "";

    public static IPacketData Deserialize(string serializedPacketData) => new PacketLogout();
}
