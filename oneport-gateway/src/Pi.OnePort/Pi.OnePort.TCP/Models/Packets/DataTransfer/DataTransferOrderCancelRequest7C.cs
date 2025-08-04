using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderCancelRequest7C : IDataTransferPacketInBound
{
    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }

    public MessageType MessageType { get; } = MessageType.OrderCancelRequest7C;

    public string Serialize()
    {
        var format = "{0,-" + FieldLength.MessageType + "}" +
            "{1,-" + FieldLength.RefOrderId + "}" +
            "{2,-" + FieldLength.FisOrderId + "}" +
            "{3,-" + FieldLength.EnterId + "}";

        return string.Format(
            format,
            MessageType.GetSerializedValue(),
            RefOrderId,
            FisOrderId,
            EnterId
        );
    }
}
