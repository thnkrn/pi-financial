using Pi.OnePort.TCP.Enums;

namespace Pi.OnePort.TCP.Models.Packets;

public interface IPacketData
{
    PacketType PacketType { get; }
}
