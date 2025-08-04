using System.Globalization;
using System.Text;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Helpers;
using Pi.OnePort.TCP.Models.Packets;

namespace Pi.OnePort.TCP.Models;

// Packet
//+-----------------+--------+--------+
//| Field Name      | Type   | Size   |
//+-----------------+--------+--------+
//| Standard Header |        |        |
//+-----------------+--------+--------+
//| Data            | String | 0-512  |
//+-----------------+--------+--------+
//| Standard Trailer|        |        |
//+-----------------+--------+--------+
public record Packet
{
    public Packet(IPacketData packetData)
    {
        Data = packetData;
    }

    private const int FooterFieldLength = 1;
    public int Sequence { get; init; }
    public DateTime? Timestamp { get; init; } = DateTime.UtcNow;
    public static readonly Encoding Encoding = Encoding.UTF8;

    public IPacketData Data { get; }

    public byte[] Serialize()
    {
        if (Data is not IPacketDataInBound)
        {
            throw new InvalidDataException();
        }

        var payload = ((IPacketDataInBound)Data).Serialize();
        var header = new StandardHeader
        {
            Sequence = Sequence,
            PkgType = Data.PacketType,
            PkgTime = Timestamp ?? DateTime.UtcNow
        }; ;
        var standardHeader = header.Serialize();
        var content = $"{standardHeader}{payload}{DataFormat.Etx}";

        return Encoding.GetBytes(content);
    }

    public static Packet Deserialize(byte[] serializedPacket)
    {
        return Deserialize(Encoding.GetString(serializedPacket));
    }

    public static Packet Deserialize(string serializedPacket)
    {
        var helper = new DeserializationHelper(serializedPacket);

        var sequence = Convert.ToInt32(helper.Next(StandardHeader.SequenceFieldLength));
        var packetType = EnumExtensions.ParseFromSerializedValue<PacketType>(helper.Next(FieldLength.Type));
        var timestamp = DateTime.ParseExact(helper.Next(DataFormat.DateTime.Length), DataFormat.DateTime, CultureInfo.InvariantCulture);

        var dataFieldLength = serializedPacket.Length - helper.End - FooterFieldLength;
        var packetData = helper.Next(dataFieldLength);

        var data = packetType switch
        {
            PacketType.Logon => PacketLogon.Deserialize(packetData),
            PacketType.Logout => PacketLogout.Deserialize(packetData),
            PacketType.Heartbeat => PacketHeartbeat.Deserialize(packetData),
            PacketType.TestRequest => PacketTest.Deserialize(packetData),
            PacketType.RecoveryRequest => PacketRecoveryRequest.Deserialize(packetData),
            PacketType.RecoveryAcknowledge => PacketRecoveryAcknowledge.Deserialize(packetData),
            PacketType.RecoveryComplete => PacketRecoveryComplete.Deserialize(packetData),
            PacketType.DataTransfer => PacketDataTransfer.Deserialize(packetData),
            _ => throw new ArgumentException($"Invalid packet type, got  : '{packetType}'")
        };

        return new Packet(data)
        {
            Sequence = sequence,
            Timestamp = timestamp,
        };
    }
}
