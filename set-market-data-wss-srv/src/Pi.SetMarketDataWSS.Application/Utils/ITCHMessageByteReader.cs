using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Utils;

public class ItchMessageByteReader(Memory<byte> data) : IDisposable
{
    private readonly BinaryReader _reader = new(new MemoryStream(data.ToArray()));

    public bool EndOfStream => _reader.BaseStream.Position == _reader.BaseStream.Length;


    public void Dispose()
    {
        _reader?.Dispose();
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
        return new Numeric16(bytes);
    }

    public Numeric32 ReadNumeric32()
    {
        var bytes = _reader.ReadBytes(4);
        return new Numeric32(bytes);
    }

    public Numeric64 ReadNumeric64()
    {
        var bytes = _reader.ReadBytes(8);
        return new Numeric64(bytes);
    }

    public Numeric96 ReadNumeric96()
    {
        var bytes = _reader.ReadBytes(12);
        return new Numeric96(bytes);
    }

    public Price32 ReadPrice32()
    {
        var bytes = _reader.ReadBytes(4);
        return new Price32(bytes);
    }

    public Price64 ReadPrice64()
    {
        var bytes = _reader.ReadBytes(8);
        return new Price64(bytes);
    }

    public Date ReadDate()
    {
        var bytes = _reader.ReadBytes(4);
        return new Date(bytes);
    }

    public Time ReadTime()
    {
        var bytes = _reader.ReadBytes(4);
        return new Time(bytes);
    }

    public Timestamp ReadTimestamp()
    {
        var bytes = _reader.ReadBytes(8);
        return new Timestamp(bytes);
    }
}