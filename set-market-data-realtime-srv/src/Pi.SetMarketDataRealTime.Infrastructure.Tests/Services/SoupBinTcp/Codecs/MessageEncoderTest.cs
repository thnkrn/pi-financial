using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Codecs;
using Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Services.SoupBinTcp.Codecs;

public class MessageEncoderTests
{
    private readonly EmbeddedChannel _channel = new(new MessageEncoder());

    [Fact]
    public void Encode_ProducesCorrectIByteBuffer()
    {
        byte[]? testData = [0x01, 0x02, 0x03];
        var testMessage = new TestMessage(testData);

        _channel.WriteOutbound(testMessage);

        var outboundMessage = _channel.ReadOutbound<IByteBuffer>();
        Assert.NotNull(outboundMessage);

        var receivedData = new byte[testData.Length];
        outboundMessage.ReadBytes(receivedData);
        Assert.Equal(testData, receivedData);

        outboundMessage.Release();
    }
}