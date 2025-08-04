using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketRecoveryAcknowledge : IPacketDataInBound, IPacketDataOutBound
{
    public required int BeginSequenceNumber { get; init; }
    public required int EndSequenceNumber { get; init; }
    public PacketType PacketType => PacketType.RecoveryAcknowledge;

    private static readonly string SerializedFormat =
        $"{{0,{-FieldLength.BeginSequenceNumber}:D{FieldLength.BeginSequenceNumber}}}{{1,{-FieldLength.EndSequenceNumber}:D{FieldLength.EndSequenceNumber}}}";

    public string Serialize() => string.Format(SerializedFormat, BeginSequenceNumber, EndSequenceNumber);

    public static IPacketData Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new PacketRecoveryAcknowledge
        {
            BeginSequenceNumber = Convert.ToInt32(helper.Next(FieldLength.BeginSequenceNumber)),
            EndSequenceNumber = Convert.ToInt32(helper.Next(FieldLength.EndSequenceNumber))
        };
    }
}
