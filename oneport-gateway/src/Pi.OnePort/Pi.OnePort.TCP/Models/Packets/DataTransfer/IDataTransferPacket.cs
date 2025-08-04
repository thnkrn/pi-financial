using Pi.OnePort.TCP.Enums.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public interface IDataTransferPacket
{
    MessageType MessageType { get; }
}
