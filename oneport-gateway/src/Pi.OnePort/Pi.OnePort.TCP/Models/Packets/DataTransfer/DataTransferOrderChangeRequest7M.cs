using System.ComponentModel.DataAnnotations;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferOrderChangeRequest7M : IDataTransferPacketInBound
{
    public required string RefOrderId { get; init; }
    public required string FisOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string Account { get; init; }
    public required string Client { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderType OrderType { get; init; }
    [Required]
    public required decimal Price { get; init; }
    public required ConPrice ConPrice { get; init; }
    [Required]
    public required int Volume { get; init; }
    [Required]
    public required int PublishVol { get; init; }

    public MessageType MessageType { get; } = MessageType.OrderChangeRequest7M;

    public string Serialize()
    {
        var format = "{0,-" + FieldLength.MessageType + "}" +
            "{1,-" + FieldLength.RefOrderId + "}" +
            "{2,-" + FieldLength.FisOrderId + "}" +
            "{3,-" + FieldLength.EnterId + "}" +
            "{4,-" + FieldLength.Account + "}" +
            "{5,-" + FieldLength.PortClient + "}" +
            "{6,-" + FieldLength.Ttf + "}" +
            "{7,-" + FieldLength.OrderType + "}" +
            "{8," + FieldLength.Price + ":F6}" +
            "{9,-" + FieldLength.ConPrice + "}" +
            "{10:D" + FieldLength.Volume + "}" +
            "{11:D" + FieldLength.PublishVolume + "}";

        return string.Format(
            format,
            MessageType.GetSerializedValue(),
            RefOrderId,
            FisOrderId,
            EnterId,
            Account,
            Client,
            Ttf.GetSerializedValue(),
            OrderType.GetSerializedValue(),
            Price,
            ConPrice.GetSerializedValue(),
            Volume,
            PublishVol
        );
    }
}
