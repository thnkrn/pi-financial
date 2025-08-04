using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketRecoveryRequest : IPacketDataInBound, IPacketDataOutBound
{
    public required int LastSequenceNumber { get; init; }
    public required int BeginSequenceNumber { get; init; }
    public required int EndSequenceNumber { get; init; }
    public PacketType PacketType => PacketType.RecoveryRequest;

    private static readonly string SerializedFormat =
        $"{{0,{-FieldLength.LastSequenceNumber}:D{FieldLength.LastSequenceNumber}}}{{1,{-FieldLength.BeginSequenceNumber}:D{FieldLength.BeginSequenceNumber}}}{{2,{-FieldLength.EndSequenceNumber}:D{FieldLength.EndSequenceNumber}}}";

    public string Serialize() => string.Format(SerializedFormat, LastSequenceNumber, BeginSequenceNumber, EndSequenceNumber);

    public static IPacketData Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new PacketRecoveryRequest
        {
            LastSequenceNumber = Convert.ToInt32(helper.Next(FieldLength.LastSequenceNumber)),
            BeginSequenceNumber = Convert.ToInt32(helper.Next(FieldLength.BeginSequenceNumber)),
            EndSequenceNumber = Convert.ToInt32(helper.Next(FieldLength.EndSequenceNumber))
        };
    }
}
