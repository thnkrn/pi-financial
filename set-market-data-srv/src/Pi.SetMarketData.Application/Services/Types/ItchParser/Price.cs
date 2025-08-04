namespace Pi.SetMarketData.Application.Services.Types.ItchParser;

/// <summary>
/// Actual price value in integer
/// </summary>
public struct Price
{
    /// <summary>
    /// Actual value of price
    /// </summary>
    public int Value { get; set; }
    /// <summary>
    /// Number of decimals point of price value
    /// </summary>
    public int NumberOfDecimals { get; set; }

    public Price(int value)
    {
        Value = value;
    }
}

/// <summary>
/// Price value that will be initiated by 4 byte data
/// </summary>
public struct Price32
{
    private const int NoPriceIndicator = -2147483648;
    public int Value { get; set; }
    public int NumberOfDecimals { get; set; }

    public Price32(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
        {
            throw new ArgumentException("bytes must be exactly 4 bytes long for Price32.");
        }

        // TODO: confirm Price uses big-endian encoding
        if (BitConverter.IsLittleEndian)
        {
            // If system is little-endian, reverse the byte array to interpret it as big-endian
            byte[] reversedBytes = bytes.ToArray();
            Array.Reverse(reversedBytes);
            Value = BitConverter.ToInt32(reversedBytes, 0);
        }
        else
        {
            Value = BitConverter.ToInt32(bytes);
        }
    }

    public Price32(int value)
    {
        Value = value;
    }

    public decimal ToFloat()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
        {
            throw new InvalidOperationException("No price available.");
        }

        return Value / (decimal)Math.Pow(10, NumberOfDecimals);
    }

    public static implicit operator decimal(Price32 price)
    {
        return price.ToFloat();
    }

    public override string ToString()
    {
        return ToFloat().ToString($"F{NumberOfDecimals}");
    }
}

/// <summary>
/// Price value that will be initiated by 8 byte data
/// </summary>
public struct Price64
{
    private static readonly long NoPriceIndicator = -2147483648;
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }

    public Price64(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
        {
            throw new ArgumentException("bytes must be exactly 8 bytes long for Price64.");
        }

        // TODO: confirm Price uses big-endian encoding
        if (BitConverter.IsLittleEndian)
        {
            // If system is little-endian, reverse the byte array to interpret it as big-endian
            byte[] reversedBytes = bytes.ToArray();
            Array.Reverse(reversedBytes);
            Value = BitConverter.ToInt64(reversedBytes, 0);
        }
        else
        {
            Value = BitConverter.ToInt64(bytes);
        }
    }

    public Price64(int value)
    {
        Value = value;
    }

    public decimal ToFloat()
    {
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
        {
            throw new InvalidOperationException("No price available.");
        }

        return Value / (decimal)Math.Pow(10, NumberOfDecimals);
    }

    public static implicit operator decimal(Price64 price)
    {
        return price.ToFloat();
    }

    public override string ToString()
    {
        return ToFloat().ToString($"F{NumberOfDecimals}");
    }
}