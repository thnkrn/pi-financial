namespace Pi.OnePort.TCP.Models.Packets;

public interface IPacketDataInBound : IPacketData
{
    string Serialize();
}
