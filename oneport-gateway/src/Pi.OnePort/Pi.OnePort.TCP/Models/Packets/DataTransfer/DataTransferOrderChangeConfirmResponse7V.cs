using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangeConfirmResponse7V : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.OrderChangeConfirmResponse7V;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string Account { get; init; }
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderStatus OrderStatus { get; init; }
    public required int? ExecutionTransRejectType { get; init; }
    public required Source Source { get; init; }
    public required string Reason { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);
        var refOrderId = helper.Next(FieldLength.RefOrderId).Trim();
        var fisOrderId = helper.Next(FieldLength.FisOrderId).Trim();
        var enterId = helper.Next(FieldLength.EnterId).Trim();
        var account = helper.Next(FieldLength.Account).Trim();
        var executionTransType = EnumExtensions.ParseFromSerializedValue<ExecutionTransType>(helper.Next(FieldLength.ExecutionTransType));
        var ttf = EnumExtensions.ParseFromSerializedValue<Ttf>(helper.Next(FieldLength.Ttf));
        var orderStatus = EnumExtensions.ParseFromSerializedValue<OrderStatus>(helper.Next(FieldLength.OrderStatus));
        var executionTransRejectTypeStr = helper.Next(FieldLength.ExecutionTransRejectType);
        var source = EnumExtensions.ParseFromSerializedValue<Source>(helper.Next(FieldLength.SourceId));
        var reason = helper.Next(FieldLength.ReasonText).Trim();

        return new DataTransferOrderChangeConfirmResponse7V
        {
            RefOrderId = refOrderId,
            FisOrderId = fisOrderId,
            EnterId = enterId,
            Account = account,
            ExecutionTransType = executionTransType,
            Ttf = ttf,
            OrderStatus = orderStatus,
            ExecutionTransRejectType = int.TryParse(executionTransRejectTypeStr, out var executionTransRejectType)
                ? executionTransRejectType
                : null,
            Source = source,
            Reason = reason
        };
    }
}
