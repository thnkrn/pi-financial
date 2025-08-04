using System.Buffers.Binary;
using System.Globalization;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

public struct Timestamp : IEquatable<Timestamp>, IComparable<Timestamp>
{
    private const long NanosecondsPerSecond = 1_000_000_000;
    private const long NanosecondsPerMillisecond = 1_000_000;
    private readonly long _unixNanoseconds;

    public Timestamp(long unixNanoseconds)
    {
        _unixNanoseconds = unixNanoseconds;
    }

    public Timestamp(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
            throw new ArgumentException("Bytes must be exactly 8 bytes long.", nameof(bytes));

        _unixNanoseconds = BitConverter.IsLittleEndian
            ? BinaryPrimitives.ReverseEndianness(BitConverter.ToInt64(bytes))
            : BitConverter.ToInt64(bytes);
    }

    public static Timestamp Parse(long unixNanoseconds)
    {
        return new Timestamp(unixNanoseconds);
    }

    public static Timestamp Parse(string timestampString)
    {
        if (string.IsNullOrEmpty(timestampString))
            throw new ArgumentNullException(nameof(timestampString));

        timestampString = timestampString.TrimEnd(' ', 'U', 'T', 'C');

        var parts = timestampString.Split('.');
        if (parts.Length != 2)
            throw new FormatException($"Invalid timestamp format: {timestampString}");

        if (!DateTime.TryParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dateTime))
            throw new FormatException($"Unable to parse date part: {parts[0]}");

        if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var fractionalSeconds))
            throw new FormatException($"Unable to parse fractional seconds: {parts[1]}");

        var unixNanoseconds =
            (long)(dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds * NanosecondsPerSecond +
            fractionalSeconds;

        return new Timestamp(unixNanoseconds);
    }

    public void Deconstruct(out DateTime dateTime, out long extraPrecision)
    {
        (dateTime, extraPrecision) = GetDateTimeAndExtraPrecision();
    }

    public static long ConvertToUnixTimestampMilliseconds(string utcTimestamp)
    {
        var timestamp = Parse(utcTimestamp);
        return timestamp._unixNanoseconds / NanosecondsPerMillisecond;
    }

    public override string ToString()
    {
        var (dateTime, extraPrecision) = GetDateTimeAndExtraPrecision();
        return Formatter(dateTime, extraPrecision);
    }

    private (DateTime dateTime, long extraPrecision) GetDateTimeAndExtraPrecision()
    {
        var seconds = _unixNanoseconds / NanosecondsPerSecond;
        var nanoseconds = _unixNanoseconds % NanosecondsPerSecond;
        var dateTime = DateTime.UnixEpoch.AddSeconds(seconds);

        return (dateTime, nanoseconds);
    }

    public static string Formatter(DateTime dateTime, long extraPrecision)
    {
        var formattedDateTime = dateTime.ToString(
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture
        );
        return $"{formattedDateTime}.{extraPrecision.ToString().PadLeft(9, '0')} UTC";
    }

    public long ToUnixTimestampMilliseconds()
    {
        return _unixNanoseconds / NanosecondsPerMillisecond;
    }

    public bool Equals(Timestamp other)
    {
        return _unixNanoseconds == other._unixNanoseconds;
    }

    public override bool Equals(object? obj)
    {
        return obj is Timestamp other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _unixNanoseconds.GetHashCode();
    }

    public int CompareTo(Timestamp other)
    {
        return _unixNanoseconds.CompareTo(other._unixNanoseconds);
    }

    public static bool operator ==(Timestamp left, Timestamp right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Timestamp left, Timestamp right)
    {
        return !(left == right);
    }

    public static bool operator <(Timestamp left, Timestamp right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Timestamp left, Timestamp right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Timestamp left, Timestamp right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Timestamp left, Timestamp right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static explicit operator DateTimeOffset(Timestamp timestamp)
    {
        var seconds = timestamp._unixNanoseconds / NanosecondsPerSecond;
        var nanos = timestamp._unixNanoseconds % NanosecondsPerSecond;

        var dateTime = DateTime.UnixEpoch.AddSeconds(seconds);
        var ticks = nanos / 100; // Convert nanoseconds to ticks (1 tick = 100 nanoseconds)

        return new DateTimeOffset(dateTime, TimeSpan.Zero).AddTicks(ticks);
    }
}