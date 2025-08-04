using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketDataWSS.Infrastructure.Services.SoupBinTcp.Codecs;

namespace Pi.SetMarketDataWSS.Infrastructure.Tests.Services.SoupBinTcp.Codecs;

public class MessageDecoderTests
{
    private readonly EmbeddedChannel _channel;

    public MessageDecoderTests()
    {
        var decoder = new MessageDecoder();
        _channel = new EmbeddedChannel(decoder);
    }

    private static IByteBuffer CreateByteBufWithFirstByte(byte type)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(type);
        buffer.WriteBytes(new byte[] { 0x00, 0x01, 0x02 });
        return buffer;
    }

    [Theory]
    [InlineData(43, typeof(Debug))]
    [InlineData(76, typeof(LoginRequest))]
    [InlineData(65, typeof(LoginAccepted))]
    [InlineData(74, typeof(LoginRejected))]
    [InlineData(79, typeof(LogoutRequest))]
    [InlineData(83, typeof(SequencedData))]
    [InlineData(85, typeof(UnSequencedData))]
    [InlineData(72, typeof(ServerHeartbeat))]
    [InlineData(82, typeof(ClientHeartbeat))]
    public void Decode_CorrectTypeBasedOnByte(byte typeByte, System.Type expectedType)
    {
        var input = CreateByteBufWithFirstByte(typeByte);

        _channel.WriteInbound(input);

        var decoded = _channel.ReadInbound<object>();

        Assert.NotNull(decoded);
        Assert.IsType(expectedType, decoded);
    }
}