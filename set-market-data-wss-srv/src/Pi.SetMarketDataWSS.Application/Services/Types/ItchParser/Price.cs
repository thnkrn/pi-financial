namespace Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

// ReSharper disable DefaultStructEqualityIsUsed.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// 4-byte Price
public struct Price32
{
    private const int NoPriceIndicator = -2147483648;
    public int Value { get; set; }
    public int NumberOfDecimals { get; set; }

    public Price32(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4) throw new ArgumentException("bytes must be exactly 4 bytes long for Price32.");
        
        if (BitConverter.IsLittleEndian)
        {
            // If system is little-endian, reverse the byte array to interpret it as big-endian
            var reversedBytes = bytes.ToArray();
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
            throw new InvalidOperationException("No price available.");

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

    public static bool HasValue(int? value)
    {
        return value != NoPriceIndicator && value != null;
    }
}

// 8-byte Price
public struct Price64
{
    private const long NoPriceIndicator = -2147483648;
    public long Value { get; set; }
    public int NumberOfDecimals { get; set; }
    private bool HandleSentinelValue => true;

    public Price64(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
            throw new ArgumentException("bytes must be exactly 8 bytes long for Price64.");

        // Confirm Price uses big-endian encoding
        if (BitConverter.IsLittleEndian)
        {
            // If system is little-endian, reverse the byte array to interpret it as big-endian
            var reversedBytes = bytes.ToArray();
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
        // Return 0 for the sentinel value
        if (Value == NoPriceIndicator && NumberOfDecimals == 0)
            if (HandleSentinelValue)
                return 0m;
            else
                throw new InvalidOperationException("No price available.");

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
    
    public static bool HasValue(long? value)
    {
        return value != NoPriceIndicator && value != null;
    }
}