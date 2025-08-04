namespace Pi.OnePort.TCP.Models.Packets;

public interface IPacketDataOutBound : IPacketData
{
    static abstract IPacketData Deserialize(string serializedPacketData);
}
