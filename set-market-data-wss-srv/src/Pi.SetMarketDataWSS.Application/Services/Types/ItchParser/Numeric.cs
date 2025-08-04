using System.Numerics;

namespace Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

public struct Numeric8
{
    public byte Value { get; set; }

    public Numeric8(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 1) throw new ArgumentException("bytes must be exactly 1 byte long.");

        Value = bytes[0];
    }

    public Numeric8(byte value)
    {
        Value = value;
    }

    public static implicit operator byte(Numeric8 numeric8)
    {
        return numeric8.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public struct Numeric16
{
    public ushort Value { get; set;}

    public Numeric16(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 2) throw new ArgumentException("bytes must be exactly 2 bytes long.");

        Array.Reverse(bytes); // Reverse for big-endian to little-endian conversion
        Value = BitConverter.ToUInt16(bytes, 0);
    }

    public static implicit operator ushort(Numeric16 numeric16)
    {
        return numeric16.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public struct Numeric32
{
    public uint Value { get; set; }

    public Numeric32(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 4) throw new ArgumentException("bytes must be exactly 4 bytes long.");

        // Ensure big-endian encoding
        Array.Reverse(bytes);
        Value = BitConverter.ToUInt32(bytes, 0);
    }

    public static implicit operator uint(Numeric32 numeric32)
    {
        return numeric32.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public struct Numeric64
{
    public ulong Value { get; set; }

    public Numeric64(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 8) throw new ArgumentException("bytes must be exactly 8 bytes long.");

        Array.Reverse(bytes);
        Value = BitConverter.ToUInt64(bytes, 0);
    }

    public static implicit operator ulong(Numeric64 numeric64)
    {
        return numeric64.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public struct Numeric96
{
    public byte[] Value { get; }

    public Numeric96(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 12) throw new ArgumentException("bytes must be exactly 12 bytes long.");

        Value = new byte[12];
        Array.Copy(bytes, Value, 12);
    }

    public override string ToString()
    {
        return BitConverter.ToString(Value).Replace("-", string.Empty);
    }

    public static implicit operator BigInteger(Numeric96 numeric96)
    {
        // Ensure the BigInteger is constructed in a way that avoids sign interpretation issues.
        // Prepend a zero byte to ensure the value is interpreted as positive.
        var temp = new byte[numeric96.Value.Length + 1];
        Array.Copy(numeric96.Value, 0, temp, 1, numeric96.Value.Length);
        return new BigInteger(temp, true, true);
    }
}