using System.Globalization;

namespace Pi.SetMarketData.Application.Services.Types.ItchParser;

public struct Date
{
    private readonly DateTime value;

    // Constructor accepting an integer
    public Date(uint dateInt)
    {
        value = Parse(dateInt);
    }

    // Constructor accepting a ReadOnlySpan<byte> in big endian
    public Date(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
        {
            throw new ArgumentException("Bytes must be exactly 4 bytes long.");
        }
        uint dateInt = new Numeric32(bytes.ToArray());
        value = Parse(dateInt);
    }

    private static DateTime Parse(uint dateInt)
    {
        int year = (int)dateInt / 10000;
        int month = (int)(dateInt / 100) % 100;
        int day = (int)dateInt % 100;

        try
        {
            //TODO: clarify how to handle case of value = 0
            if (dateInt == 0)
            {
                return DateTime.MinValue;
            }

            return new DateTime(year, month, day);
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new ArgumentException("Invalid date format", nameof(dateInt), e);
        }
    }

    public override string ToString() =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static implicit operator DateTime(Date date) => date.value;
}