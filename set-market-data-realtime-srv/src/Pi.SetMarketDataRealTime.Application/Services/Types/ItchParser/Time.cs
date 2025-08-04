using System.Buffers.Binary;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

public struct Time : IEquatable<Time>
{
    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }

    public Time(int timeInt)
    {
        Hours = timeInt / 10000;
        Minutes = timeInt / 100 % 100;
        Seconds = timeInt % 100;

        ValidateTime();
    }

    public Time(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Bytes must be exactly 4 bytes long.", nameof(bytes));

        var timeInt = BinaryPrimitives.ReadInt32BigEndian(bytes);

        Hours = timeInt / 10000;
        Minutes = timeInt / 100 % 100;
        Seconds = timeInt % 100;

        ValidateTime();
    }

    private void ValidateTime()
    {
        if (Hours < 0 || Hours > 23 || Minutes < 0 || Minutes > 59 || Seconds < 0 || Seconds > 59)
            throw new ArgumentException("Invalid time value.");
    }

    public override string ToString()
    {
        return $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}";
    }

    public string ToString(string format)
    {
        return TimeOnly.FromTimeSpan(ToTimeSpan()).ToString(format);
    }

    public TimeSpan ToTimeSpan()
    {
        return new TimeSpan(Hours, Minutes, Seconds);
    }

    public static implicit operator TimeSpan(Time time)
    {
        return time.ToTimeSpan();
    }

    public bool Equals(Time other)
    {
        return Hours == other.Hours && Minutes == other.Minutes && Seconds == other.Seconds;
    }

    public override bool Equals(object? obj)
    {
        return obj is Time other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Hours, Minutes, Seconds);
    }

    public static bool operator ==(Time left, Time right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Time left, Time right)
    {
        return !left.Equals(right);
    }
}