using System.Globalization;
using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums;
using Pi.OnePort.TCP.Extensions;

namespace Pi.OnePort.TCP.Models;

// StandardHeader
//+------------+---------------------+-----------+--------+-----------------------------------------+
//| Field Name | Type                | Type      | Size   | Comment                                 |
//+------------+---------------------+-----------+--------+-----------------------------------------+
//| Length     | Short Network Order |           | 2      | Lengths of package exclude field length.|
//+------------+---------------------+-----------+--------+-----------------------------------------+
//| Sequence   | Int                 | int       | 8      | 00000000 – 99999999                     |
//+------------+---------------------+-----------+--------+-----------------------------------------+
//| PkgType    | String              | string    | 2      | Type of package                         |
//+------------+---------------------+-----------+--------+-----------------------------------------+
//| PkgTime    | Time                | time.Time | 15     | YYYYMMDD-HHMMSS                         |
//+------------+---------------------+-----------+--------+-----------------------------------------+
public record StandardHeader
{
    public const int SequenceFieldLength = 8;
    private static readonly string HeaderSerializedFormat = $"{{0,{-SequenceFieldLength}:D{SequenceFieldLength}}}{{1,{FieldLength.Type}}}{{2,{-DataFormat.DateTime.Length}}}";

    public required int Sequence { get; init; }
    public required PacketType PkgType { get; init; }
    public required DateTime PkgTime { get; init; }

    public string Serialize()
    {
        var timestamp = PkgTime.ToString(DataFormat.DateTime, CultureInfo.InvariantCulture);

        return string.Format(HeaderSerializedFormat, Sequence, PkgType.GetSerializedValue(), timestamp);
    }
}
