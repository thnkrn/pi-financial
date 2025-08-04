using System.Net;
using DotNetty.Transport.Channels;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

public class ReconnectHandler(Func<EndPoint, Task> doConnectFunc) : ChannelHandlerAdapter
{
    public override void ChannelInactive(IChannelHandlerContext context)
    {
        base.ChannelInactive(context);
        context.Channel.EventLoop.Schedule(connect => doConnectFunc((EndPoint)connect), context.Channel.RemoteAddress,
            TimeSpan.FromMilliseconds(1000));
    }
}