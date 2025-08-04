using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangeResponse7N : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.OrderChangeResponse7N;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string Account { get; init; }
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required Ttf Ttf { get; init; }
    public required decimal Price { get; init; }
    public required ConPrice ConPrice { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PublishVolume { get; init; }
    public required OrderStatus OrderStatus { get; init; }
    public required ExecutionTransRejectType? ExecutionTransRejectType { get; init; }
    public required Source Source { get; init; }
    public required string Reason { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new DataTransferOrderChangeResponse7N
        {
            RefOrderId = helper.Next(FieldLength.RefOrderId).Trim(),
            FisOrderId = helper.Next(FieldLength.FisOrderId).Trim(),
            EnterId = helper.Next(FieldLength.EnterId).Trim(),
            Account = helper.Next(FieldLength.Account).Trim(),
            ExecutionTransType = EnumExtensions.ParseFromSerializedValue<ExecutionTransType>(helper.Next(FieldLength.ExecutionTransType)),
            Ttf = EnumExtensions.ParseFromSerializedValue<Ttf>(helper.Next(FieldLength.Ttf)),
            Price = decimal.TryParse(helper.Next(FieldLength.Price), out var parsedPrice)
                ? parsedPrice
                : 0,
            ConPrice = EnumExtensions.ParseFromSerializedValue<ConPrice>(helper.Next(FieldLength.ConPrice)),
            Volume = decimal.Parse(helper.Next(FieldLength.Volume)),
            PublishVolume = decimal.TryParse(helper.Next(FieldLength.PublishVolume), out var parsedPublishVolume)
                ? parsedPublishVolume
                : 0,
            OrderStatus = EnumExtensions.ParseFromSerializedValue<OrderStatus>(helper.Next(FieldLength.OrderStatus)),
            ExecutionTransRejectType = helper.TryNew(FieldLength.ExecutionTransRejectType, out var parsedExecutionTransRejectType)
                ? EnumExtensions.GetEnumFromAttribute<ExecutionTransRejectType, SerializedValue>(parsedExecutionTransRejectType)
                : null,
            Source = EnumExtensions.ParseFromSerializedValue<Source>(helper.Next(FieldLength.SourceId)),
            Reason = helper.Next(FieldLength.ReasonText).Trim()
        };
    }
}
