using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketHeartbeat : IPacketDataInBound, IPacketDataOutBound
{
    public required RequestType RequestType { get; init; }
    public required string Reserve { get; init; }
    public bool ResultFlag { get; init; } = true;
    public PacketType PacketType => PacketType.Heartbeat;

    private static readonly string SerializedFormat = $"{{0,{FieldLength.Type}}}{{1,-{FieldLength.HeartbeatReserveValue}}}{{2,{FieldLength.HeartbeatResult}}}";

    public string Serialize()
    {
        return string.Format(SerializedFormat, RequestType, Reserve, CastFlag(ResultFlag));
    }

    public static IPacketData Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new PacketHeartbeat
        {
            RequestType = EnumExtensions.GetEnumFromAttribute<RequestType, SerializedValue>(helper.Next(FieldLength.Type)),
            Reserve = helper.Next(FieldLength.HeartbeatReserveValue).Trim().Replace(DataFormat.Etx, ""),
            ResultFlag = helper.Next(FieldLength.HeartbeatResult) == "0"
        };
    }

    private static short CastFlag(bool flag) => (short)(flag ? 0 : 1);
}
