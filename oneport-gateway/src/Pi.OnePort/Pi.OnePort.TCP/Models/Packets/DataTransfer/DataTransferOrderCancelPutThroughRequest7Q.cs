using Pi.OnePort.TCP.Enums.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderCancelPutThroughRequest7Q : IDataTransferPacketInBound
{
    public MessageType MessageType => MessageType.OrderCancelPutThroughRequest7Q;

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}
