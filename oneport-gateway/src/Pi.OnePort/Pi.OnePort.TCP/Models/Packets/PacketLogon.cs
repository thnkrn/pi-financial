using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Helpers;

namespace Pi.OnePort.TCP.Models.Packets;

public record PacketLogon : IPacketDataInBound, IPacketDataOutBound
{
    public required string LoginId { get; init; }
    public required string Password { get; init; }
    public int HeartBeat { get; init; } = 20;

    public PacketType PacketType { get; } = PacketType.Logon;

    public string Serialize()
    {
        return $"{HeartBeat:0000}{LoginId,-20}{Password,-20}";
    }

    public static IPacketData Deserialize(string serializedPacketData)
    {
        var helper = new DeserializationHelper(serializedPacketData);

        return new PacketLogon
        {
            HeartBeat = Convert.ToInt32(helper.Next(FieldLength.HeartbeatInterval)),
            LoginId = helper.Next(FieldLength.Username).Trim(),
            Password = helper.Next(FieldLength.Password).Trim()
        };
    }
}
