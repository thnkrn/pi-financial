using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

[Serializable]
public struct Date : ISerializable, IEquatable<Date>, IComparable<Date>
{
    private readonly DateOnly _value;

    public int Year => _value.Year;
    public int Month => _value.Month;
    public int Day => _value.Day;

    public Date(uint dateInt)
    {
        _value = ParseFromInt(dateInt);
    }

    public Date(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Bytes must be exactly 4 bytes long.", nameof(bytes));

        var dateInt = BinaryPrimitives.ReadUInt32BigEndian(bytes);
        _value = ParseFromInt(dateInt);
    }

    public Date(string dateString)
    {
        if (!DateOnly.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var result))
            throw new ArgumentException("Invalid date string. Format should be 'yyyyMMdd'.", nameof(dateString));
        _value = result;
    }

    private Date(SerializationInfo info, StreamingContext context)
    {
        _value = DateOnly.FromDateTime(info.GetDateTime("Value"));
    }

    private static DateOnly ParseFromInt(uint dateInt)
    {
        if (dateInt == 0)
            return DateOnly.MinValue;

        var year = (int)(dateInt / 10000);
        var month = (int)(dateInt / 100 % 100);
        var day = (int)(dateInt % 100);

        try
        {
            return new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentException($"Invalid date: {year:D4}-{month:D2}-{day:D2}", nameof(dateInt), ex);
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Value", _value.ToDateTime(TimeOnly.MinValue));
    }

    public uint ToUInt32()
    {
        return (uint)(_value.Year * 10000 + _value.Month * 100 + _value.Day);
    }

    public override string ToString()
    {
        return _value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public static implicit operator DateTime(Date date)
    {
        return date._value.ToDateTime(TimeOnly.MinValue);
    }

    public static implicit operator DateOnly(Date date)
    {
        return date._value;
    }

    public bool Equals(Date other)
    {
        return _value.Equals(other._value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Date other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public int CompareTo(Date other)
    {
        return _value.CompareTo(other._value);
    }

    public static bool operator ==(Date left, Date right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Date left, Date right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(Date left, Date right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Date left, Date right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Date left, Date right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Date left, Date right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static Date Parse(string s)
    {
        return new Date(s);
    }

    public static bool TryParse(string s, out Date result)
    {
        if (DateOnly.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateOnly))
        {
            result = new Date(dateOnly.ToString("yyyyMMdd"));
            return true;
        }

        result = default;
        return false;
    }
}