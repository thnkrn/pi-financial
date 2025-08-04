using System.Buffers.Binary;
using System.Numerics;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

public struct Numeric8 : IEquatable<Numeric8>
{
    public byte Value { get; }

    public Numeric8(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 1)
            throw new ArgumentException("Bytes must be exactly 1 byte long.", nameof(bytes));

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

    public bool Equals(Numeric8 other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Numeric8 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public struct Numeric16 : IEquatable<Numeric16>
{
    public ushort Value { get; }

    public Numeric16(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 2)
            throw new ArgumentException("Bytes must be exactly 2 bytes long.", nameof(bytes));

        Value = BinaryPrimitives.ReadUInt16BigEndian(bytes);
    }

    public Numeric16(ushort value)
    {
        Value = value;
    }

    public static implicit operator ushort(Numeric16 numeric16)
    {
        return numeric16.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool Equals(Numeric16 other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Numeric16 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public struct Numeric32 : IEquatable<Numeric32>
{
    public uint Value { get; }

    public Numeric32(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 4)
            throw new ArgumentException("Bytes must be exactly 4 bytes long.", nameof(bytes));

        Value = BinaryPrimitives.ReadUInt32BigEndian(bytes);
    }

    public Numeric32(uint value)
    {
        Value = value;
    }

    public static implicit operator uint(Numeric32 numeric32)
    {
        return numeric32.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool Equals(Numeric32 other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Numeric32 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public struct Numeric64 : IEquatable<Numeric64>
{
    public ulong Value { get; }

    public Numeric64(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 8)
            throw new ArgumentException("Bytes must be exactly 8 bytes long.", nameof(bytes));

        Value = BinaryPrimitives.ReadUInt64BigEndian(bytes);
    }

    public Numeric64(ulong value)
    {
        Value = value;
    }

    public static implicit operator ulong(Numeric64 numeric64)
    {
        return numeric64.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool Equals(Numeric64 other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Numeric64 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public struct Numeric96 : IEquatable<Numeric96>
{
    private readonly byte[] _value;

    public ReadOnlySpan<byte> Value => _value;

    public Numeric96(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 12)
            throw new ArgumentException("Bytes must be exactly 12 bytes long.", nameof(bytes));

        _value = bytes.ToArray();
    }

    public override string ToString()
    {
        return BitConverter.ToString(_value).Replace("-", string.Empty);
    }

    public static implicit operator BigInteger(Numeric96 numeric96)
    {
        Span<byte> temp = stackalloc byte[numeric96._value.Length + 1];
        numeric96._value.CopyTo(temp.Slice(1));
        return new BigInteger(temp, true, true);
    }

    public bool Equals(Numeric96 other)
    {
        return _value.AsSpan().SequenceEqual(other._value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Numeric96 other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var b in _value) hash.Add(b);

        return hash.ToHashCode();
    }
}