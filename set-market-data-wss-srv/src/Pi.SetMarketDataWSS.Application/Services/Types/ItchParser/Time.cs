namespace Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

public struct Time
{
    public int Hours { get; }
    public int Minutes { get; }
    public int Seconds { get; }

    // Constructor accepting a 4-byte integer
    public Time(int timeInt)
    {
        Hours = timeInt / 10000;
        Minutes = timeInt / 100 % 100;
        Seconds = timeInt % 100;

        ValidateTime();
    }

    // Constructor accepting a ReadOnlySpan<byte> for 4-byte big endian time
    public Time(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Bytes must be exactly 4 bytes long.");

        var timeInt = BitConverter.IsLittleEndian
            ? BitConverter.ToInt32(bytes.ToArray().Reverse().ToArray(), 0)
            : BitConverter.ToInt32(bytes);

        Hours = timeInt / 10000;
        Minutes = timeInt / 100 % 100;
        Seconds = timeInt % 100;

        ValidateTime();
    }

    private void ValidateTime()
    {
        if (
            Hours < 0
            || Hours > 23
            || Minutes < 0
            || Minutes > 59
            || Seconds < 0
            || Seconds > 59
        )
            throw new ArgumentException("Invalid time value.");
    }

    public override string ToString()
    {
        return $"{Hours:00}:{Minutes:00}:{Seconds:00}";
    }
}