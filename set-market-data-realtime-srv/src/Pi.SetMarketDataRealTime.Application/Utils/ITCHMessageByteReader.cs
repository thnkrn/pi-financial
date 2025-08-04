using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Utils;

public sealed class ItchMessageByteReader : IDisposable
{
    private readonly BinaryReader _reader;
    private bool _disposed;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public ItchMessageByteReader(Memory<byte> data)
    {
        _reader = new BinaryReader(new MemoryStream(data.ToArray()));
    }

    public bool EndOfStream => _reader.BaseStream.Position == _reader.BaseStream.Length;

    public void Dispose()
    {
        Dispose(true);
    }

    public Alpha ReadAlpha(int length)
    {
        var bytes = _reader.ReadBytes(length);
        return new Alpha(bytes, length);
    }

    public Numeric8 ReadNumeric8()
    {
        var bytes = _reader.ReadByte();
        return new Numeric8(bytes);
    }

    public Numeric16 ReadNumeric16()
    {
        var bytes = _reader.ReadBytes(2);
        if (bytes.Length < 2) throw new EndOfStreamException("Not enough bytes to read a complete ReadNumeric16.");

        return new Numeric16(bytes);
    }

    public Numeric32 ReadNumeric32()
    {
        var bytes = _reader.ReadBytes(4);
        if (bytes.Length < 4) throw new EndOfStreamException("Not enough bytes to read a complete Numeric32.");

        return new Numeric32(bytes);
    }

    public Numeric64 ReadNumeric64()
    {
        var bytes = _reader.ReadBytes(8);
        if (bytes.Length < 8) throw new EndOfStreamException("Not enough bytes to read a complete ReadNumeric64.");

        return new Numeric64(bytes);
    }

    public Numeric96 ReadNumeric96()
    {
        var bytes = _reader.ReadBytes(12);
        if (bytes.Length < 12) throw new EndOfStreamException("Not enough bytes to read a complete ReadNumeric96.");

        return new Numeric96(bytes);
    }

    public Price32 ReadPrice32()
    {
        var bytes = _reader.ReadBytes(4);
        if (bytes.Length < 4) throw new EndOfStreamException("Not enough bytes to read a complete ReadPrice32.");

        return new Price32(bytes);
    }

    public Price64 ReadPrice64()
    {
        var bytes = _reader.ReadBytes(8);
        if (bytes.Length < 8) throw new EndOfStreamException("Not enough bytes to read a complete ReadPrice64.");

        return new Price64(bytes);
    }

    public Date ReadDate()
    {
        var bytes = _reader.ReadBytes(4);
        if (bytes.Length < 4) throw new EndOfStreamException("Not enough bytes to read a complete ReadDate.");

        return new Date(bytes);
    }

    public Time ReadTime()
    {
        var bytes = _reader.ReadBytes(4);
        if (bytes.Length < 4) throw new EndOfStreamException("Not enough bytes to read a complete ReadTime.");

        return new Time(bytes);
    }

    public Timestamp ReadTimestamp()
    {
        var bytes = _reader.ReadBytes(8);
        if (bytes.Length < 8) throw new EndOfStreamException("Not enough bytes to read a complete Timestamp.");

        return new Timestamp(bytes);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _reader.Dispose();

        _disposed = true;
    }
}