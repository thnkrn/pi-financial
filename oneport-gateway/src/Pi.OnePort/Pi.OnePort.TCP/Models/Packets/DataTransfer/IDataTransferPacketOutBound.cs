namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public interface IDataTransferPacketOutBound : IDataTransferPacket
{
    static abstract IDataTransferPacket Deserialize(string serializedPacketData);
}
