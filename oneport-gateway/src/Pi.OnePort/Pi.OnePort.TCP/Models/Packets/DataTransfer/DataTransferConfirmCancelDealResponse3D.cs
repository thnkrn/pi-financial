using System.Globalization;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;
using Pi.OnePort.TCP.Utils;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferConfirmCancelDealResponse3D : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.ConfirmCancelDealResponse3D;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required DateTime TransTime { get; init; }
    public required string SecSymbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal Volume { get; init; }
    public required int ConfirmNo { get; init; }
    public required Source Source { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new DataTransferConfirmCancelDealResponse3D
        {
            RefOrderId = helper.Next(FieldLength.RefOrderId).Trim(),
            FisOrderId = helper.Next(FieldLength.FisOrderId).Trim(),
            TransTime = DateTime.ParseExact(helper.Next(DataFormat.DateTime.Length), DataFormat.DateTime, CultureInfo.InvariantCulture),
            SecSymbol = RegexUtils.SanitizeSecSymbol(helper.Next(FieldLength.SecSymbol).Trim()),
            Side = EnumExtensions.ParseFromSerializedValue<OrderSide>(helper.Next(FieldLength.Side)),
            Volume = decimal.Parse(helper.Next(FieldLength.VolumeNew)),
            ConfirmNo = int.Parse(helper.Next(FieldLength.ConfirmNo)),
            Source = EnumExtensions.ParseFromSerializedValue<Source>(helper.Next(FieldLength.SourceId))
        };
    }
}
