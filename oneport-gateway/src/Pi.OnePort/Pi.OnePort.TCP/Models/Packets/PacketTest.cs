using Pi.OnePort.TCP.Enums;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketTest(string Str) : IPacketDataInBound, IPacketDataOutBound
{
    public PacketType PacketType => PacketType.TestRequest;
    public string Serialize() => Str;

    public static IPacketData Deserialize(string serializedPacketData) => new PacketTest(serializedPacketData.Trim());
}
