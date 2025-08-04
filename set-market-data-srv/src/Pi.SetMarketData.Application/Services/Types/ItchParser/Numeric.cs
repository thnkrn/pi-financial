using System.Numerics;

namespace Pi.SetMarketData.Application.Services.Types.ItchParser;

/// <summary>
/// Numeric value for common numeric datatype
/// </summary>
public struct Numeric
{
    public int Value { get; set; }
}

public struct Numeric8
{
    public byte Value { get; set; }

    public Numeric8(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 1)
        {
            throw new ArgumentException("bytes must be exactly 1 byte long.");
        }

        Value = bytes[0];
    }

    public Numeric8(byte value)
    {
        Value = value;
    }

    public static implicit operator byte(Numeric8 numeric8) => numeric8.Value;

    public override string ToString() => Value.ToString();
}

public struct Numeric16
{
    public ushort Value { get; private set; }

    public Numeric16(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 2)
        {
            throw new ArgumentException("bytes must be exactly 2 bytes long.");
        }

        Array.Reverse(bytes); // Reverse for big-endian to little-endian conversion
        Value = BitConverter.ToUInt16(bytes, 0);
    }

    public static implicit operator ushort(Numeric16 numeric16) => numeric16.Value;

    public override string ToString() => Value.ToString();
}

public struct Numeric32
{
    public uint Value { get; set; }

    public Numeric32(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 4)
        {
            throw new ArgumentException("bytes must be exactly 4 bytes long.");
        }

        // Ensure big-endian encoding
        Array.Reverse(bytes);
        Value = BitConverter.ToUInt32(bytes, 0);
    }

    public static implicit operator uint(Numeric32 numeric32) => numeric32.Value;

    public override string ToString() => Value.ToString();
}

public struct Numeric64
{
    public ulong Value { get; set; }

    public Numeric64(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 8)
        {
            throw new ArgumentException("bytes must be exactly 8 bytes long.");
        }

        Array.Reverse(bytes);
        Value = BitConverter.ToUInt64(bytes, 0);
    }

    public static implicit operator ulong(Numeric64 numeric64) => numeric64.Value;

    public override string ToString() => Value.ToString();
}

public struct Numeric96
{
    private readonly byte[] _value;

    public byte[] Value => _value;

    public Numeric96(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 12)
        {
            throw new ArgumentException("bytes must be exactly 12 bytes long.");
        }

        _value = new byte[12];
        Array.Copy(bytes, _value, 12);
    }

    public override string ToString()
    {
        return BitConverter.ToString(_value).Replace("-", string.Empty);
    }

    public static implicit operator BigInteger(Numeric96 numeric96)
    {
        // Ensure the BigInteger is constructed in a way that avoids sign interpretation issues.
        // Prepend a zero byte to ensure the value is interpreted as positive.
        byte[] temp = new byte[numeric96._value.Length + 1];
        Array.Copy(numeric96._value, 0, temp, 1, numeric96._value.Length);
        return new BigInteger(temp, true, true);
    }
}