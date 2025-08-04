namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public interface IDataTransferPacketInBound : IDataTransferPacket
{
    string Serialize();
}
