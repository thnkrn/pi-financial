using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;
using Pi.OnePort.TCP.Utils;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferNewOrderResponse6A : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.NewOrderResponse6A;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string SecSymbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal Price { get; init; }
    public required ConPrice ConPrice { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PublishVolume { get; init; }
    public required Conditions Condition { get; init; }
    public required string Account { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderType OrderType { get; init; }
    public required string CheckFlag { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new DataTransferNewOrderResponse6A
        {
            RefOrderId = helper.Next(FieldLength.RefOrderId).Trim(),
            FisOrderId = helper.Next(FieldLength.FisOrderId).Trim(),
            EnterId = helper.Next(FieldLength.EnterId).Trim(),
            SecSymbol = RegexUtils.SanitizeSecSymbol(helper.Next(FieldLength.SecSymbol).Trim()),
            Side = EnumExtensions.ParseFromSerializedValue<OrderSide>(helper.Next(FieldLength.Side)),
            Price = decimal.Parse(helper.Next(FieldLength.Price)),
            ConPrice = EnumExtensions.ParseFromSerializedValue<ConPrice>(helper.Next(FieldLength.ConPrice)),
            Volume = decimal.Parse(helper.Next(FieldLength.VolumeNew)),
            PublishVolume = decimal.Parse(helper.Next(FieldLength.PublishVolume)),
            Condition = EnumExtensions.ParseFromSerializedValue<Conditions>(helper.Next(FieldLength.Condition)),
            Account = helper.Next(FieldLength.Account).Trim(),
            Ttf = EnumExtensions.ParseFromSerializedValue<Ttf>(helper.Next(FieldLength.Ttf)),
            OrderType = EnumExtensions.ParseFromSerializedValue<OrderType>(helper.Next(FieldLength.OrderType)),
            CheckFlag = helper.Next(FieldLength.CheckFlag).Trim()
        };
    }
}
