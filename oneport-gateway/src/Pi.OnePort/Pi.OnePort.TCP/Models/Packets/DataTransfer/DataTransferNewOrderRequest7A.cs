using System.ComponentModel.DataAnnotations;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferNewOrderRequest7A : IDataTransferPacketInBound
{
    public required string RefOrderId { get; set; }
    public required string EnterId { get; init; }
    public required string SecSymbol { get; set; }
    public required OrderSide Side { get; set; }
    public required decimal? Price { get; set; }
    public required ConPrice ConPrice { get; set; }
    [Required]
    public required int Volume { get; set; }
    [Required]
    public required int PublishVol { get; set; }
    public required Conditions Condition { get; set; }
    public required string Account { get; set; }
    public required Ttf Ttf { get; set; }
    public required OrderType OrderType { get; set; }
    public required string CheckFlag { get; set; }
    public sbyte? ValidTillData { get; set; }

    public MessageType MessageType { get; } = MessageType.NewOrderRequest7A;

    public string Serialize()
    {
        var priceStr = $"{Price,13:0.######}";
        var format = "{0,-" + FieldLength.MessageType + "}" +
            "{1,-" + FieldLength.RefOrderId + "}" +
            "{2,-" + FieldLength.EnterId + "}" +
            "{3,-" + FieldLength.SecSymbol + "}" +
            "{4,-" + FieldLength.Side + "}" +
            "{5," + FieldLength.Price + "}" +
            "{6,-" + FieldLength.ConPrice + "}" +
            "{7:D" + FieldLength.Volume + "}" +
            "{8:D" + FieldLength.PublishVolume + "}" +
            "{9,-" + FieldLength.Condition + "}" +
            "{10,-" + FieldLength.Account + "}" +
            "{11,-" + FieldLength.Ttf + "}" +
            "{12,-" + FieldLength.OrderType + "}" +
            "{13,-" + FieldLength.CheckFlag + "}";

        var serialized = string.Format(
            format,
            MessageType.GetSerializedValue(),
            RefOrderId,
            EnterId,
            SecSymbol,
            Side.GetSerializedValue(),
            priceStr,
            ConPrice.GetSerializedValue(),
            Volume,
            PublishVol,
            Condition.GetSerializedValue(),
            Account,
            Ttf.GetSerializedValue(),
            OrderType.GetSerializedValue(),
            CheckFlag
        );

        if (ValidTillData != null)
        {
            serialized += string.Format("{0,-" + FieldLength.ValidTillData + "}", ValidTillData);
        }

        return serialized;
    }
}
