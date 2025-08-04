using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderAcknowledgementResponse7K : IDataTransferPacketOutBound
{
    public MessageType MessageType { get; } = MessageType.OrderAcknowledgementResponse7K;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required OrderStatus OrderStatus { get; init; }
    public required ExecutionTransRejectType? ExecutionTransRejectType { get; init; }
    public required Source Source { get; init; }
    public required string Reason { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {

        var helper = new DeserializationHelper(serializedPacketData);

        var refOrderId = helper.Next(FieldLength.RefOrderId).Trim();
        var fisOrderId = helper.Next(FieldLength.FisOrderId).Trim();
        var executionTransType = EnumExtensions.GetEnumFromAttribute<ExecutionTransType, SerializedValue>(helper.Next(FieldLength.ExecutionTransType));
        var orderStatus = EnumExtensions.GetEnumFromAttribute<OrderStatus, SerializedValue>(helper.Next(FieldLength.OrderStatus));
        var executionTransRejectTypeStr = helper.Next(FieldLength.ExecutionTransRejectType);
        var source = EnumExtensions.GetEnumFromAttribute<Source, SerializedValue>(helper.Next(FieldLength.SourceId));
        var reason = helper.Next(FieldLength.ReasonText).Trim();

        return new DataTransferOrderAcknowledgementResponse7K
        {
            RefOrderId = refOrderId,
            FisOrderId = fisOrderId,
            ExecutionTransType = executionTransType,
            OrderStatus = orderStatus,
            ExecutionTransRejectType = executionTransRejectTypeStr.Trim() == ""
                ? null
                : EnumExtensions.GetEnumFromAttribute<ExecutionTransRejectType, SerializedValue>(
                    executionTransRejectTypeStr),
            Source = source,
            Reason = reason
        };
    }
}
