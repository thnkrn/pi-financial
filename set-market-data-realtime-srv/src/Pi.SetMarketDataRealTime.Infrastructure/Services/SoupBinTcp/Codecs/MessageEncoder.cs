using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Codecs;

public class MessageEncoder : MessageToMessageEncoder<Message>
{
    protected override void Encode(IChannelHandlerContext context, Message message, List<object> output)
    {
        var msg = Unpooled.WrappedBuffer(message.Bytes);
        output.Add(msg);
    }
}