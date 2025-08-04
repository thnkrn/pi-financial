using Pi.OnePort.TCP.Enums.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangePutThroughRequest7U : IDataTransferPacketInBound
{
    public MessageType MessageType => MessageType.OrderChangePutThroughRequest7U;

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}
