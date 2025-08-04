using System.Globalization;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;
using Pi.OnePort.TCP.Utils;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferExecutionReportResponse7X : IDataTransferPacketOutBound
{
    public MessageType MessageType => MessageType.ExecutionReportResponse7X;

    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required DateTime TransTime { get; init; }
    public required string SecSymbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal Volume { get; init; }
    public required decimal Price { get; init; }
    public required int? ConfirmNo { get; init; }
    public required Source Source { get; init; }
    public required string MarketDealNo { get; init; }
    public required string MarketTradeNo { get; init; }

    public static IDataTransferPacket Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        var refOrderId = helper.Next(FieldLength.RefOrderId).Trim();
        var fisOrderId = helper.Next(FieldLength.FisOrderId).Trim();
        var executionTransType = EnumExtensions.ParseFromSerializedValue<ExecutionTransType>(helper.Next(FieldLength.ExecutionTransType));
        var transTime = DateTime.ParseExact(helper.Next(DataFormat.DateTime.Length), DataFormat.DateTime, CultureInfo.InvariantCulture);
        var secSymbol = RegexUtils.SanitizeSecSymbol(helper.Next(FieldLength.SecSymbol).Trim());
        var side = EnumExtensions.ParseFromSerializedValue<OrderSide>(helper.Next(FieldLength.Side));
        var volume = decimal.Parse(helper.Next(FieldLength.VolumeNew));
        var price = decimal.Parse(helper.Next(FieldLength.Price));
        var confirmNoStr = helper.Next(FieldLength.ConfirmNo);
        var source = EnumExtensions.ParseFromSerializedValue<Source>(helper.Next(FieldLength.SourceId));
        var marketDealNo = helper.Next(FieldLength.MarketDealNo);
        var marketTradeNo = helper.Next(FieldLength.MarketTradeNo);

        return new DataTransferExecutionReportResponse7X
        {
            RefOrderId = refOrderId,
            FisOrderId = fisOrderId,
            ExecutionTransType = executionTransType,
            TransTime = transTime,
            SecSymbol = secSymbol,
            Side = side,
            Volume = volume,
            Price = price,
            ConfirmNo = int.TryParse(confirmNoStr, out var confirmNo)
                ? confirmNo
                : null,
            Source = source,
            MarketDealNo = marketDealNo,
            MarketTradeNo = marketTradeNo
        };
    }
}
