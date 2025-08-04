using System.Net;
using DotNetty.Transport.Channels;

namespace Pi.SetMarketData.Infrastructure.Services.SoupBinTcp.Handlers
{
    public class ReconnectHandler(Func<EndPoint, Task> doConnectFunc) : ChannelHandlerAdapter
    {
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            context.Channel.EventLoop.Schedule(_ => doConnectFunc((EndPoint)_), context.Channel.RemoteAddress, TimeSpan.FromMilliseconds(1000));
        }
    }
}