using System.Buffers.Binary;
using System.Globalization;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

public struct Price32 : IEquatable<Price32>
{
    private const int NoPriceIndicator = -2147483648;
    public int Value { get; }
    public int NumberOfDecimals { get; set; }

    public Price32(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("bytes must be exactly 4 bytes long for Price32.", nameof(bytes));

        Value = BinaryPrimitives.ReadInt32BigEndian(bytes);
        NumberOfDecimals = 0; // Default value, can be set later
    }

    public Price32(int value)
    {
        Value = value;
        NumberOfDecimals = 0; // Default value, can be set later
    }

    public decimal ToDecimal()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
            throw new InvalidOperationException("No price available.");

        return Value / (decimal)Math.Pow(10, NumberOfDecimals);
    }

    public float ToFloat()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
            throw new InvalidOperationException("No price available.");

        return (float)(Value / Math.Pow(10, NumberOfDecimals));
    }

    public static implicit operator decimal(Price32 price)
    {
        return price.ToDecimal();
    }

    public override string ToString()
    {
        return ToDecimal().ToString($"F{NumberOfDecimals}", CultureInfo.InvariantCulture);
    }

    public bool Equals(Price32 other)
    {
        return Value == other.Value && NumberOfDecimals == other.NumberOfDecimals;
    }

    public override bool Equals(object? obj)
    {
        return obj is Price32 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, NumberOfDecimals);
    }
}

public struct Price64 : IEquatable<Price64>
{
    private const long NoPriceIndicator = -2147483648L;
    public long Value { get; }
    public int NumberOfDecimals { get; set; }

    public Price64(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
            throw new ArgumentException("bytes must be exactly 8 bytes long for Price64.", nameof(bytes));

        Value = BinaryPrimitives.ReadInt64BigEndian(bytes);
        NumberOfDecimals = 0; // Default value, can be set later
    }

    public Price64(long value)
    {
        Value = value;
        NumberOfDecimals = 0; // Default value, can be set later
    }

    public decimal ToDecimal()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
            throw new InvalidOperationException("No price available.");

        return Value / (decimal)Math.Pow(10, NumberOfDecimals);
    }

    public float ToFloat()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
            throw new InvalidOperationException("No price available.");

        return (float)(Value / Math.Pow(10, NumberOfDecimals));
    }

    public static implicit operator decimal(Price64 price)
    {
        return price.ToDecimal();
    }

    public override string ToString()
    {
        return ToDecimal().ToString($"F{NumberOfDecimals}", CultureInfo.InvariantCulture);
    }

    public bool Equals(Price64 other)
    {
        return Value == other.Value && NumberOfDecimals == other.NumberOfDecimals;
    }

    public override bool Equals(object? obj)
    {
        return obj is Price64 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, NumberOfDecimals);
    }
}