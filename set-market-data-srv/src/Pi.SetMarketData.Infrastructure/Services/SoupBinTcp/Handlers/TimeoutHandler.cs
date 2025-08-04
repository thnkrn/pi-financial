using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketData.Infrastructure.Services.SoupBinTcp.Handlers;

public class TimeoutHandler : ChannelDuplexHandler
{
    public override void UserEventTriggered(IChannelHandlerContext context, object evt)
    {
        if (evt is not IdleStateEvent e) return;
        switch (e.State)
        {
            case IdleState.WriterIdle:
                context.WriteAndFlushAsync(new ClientHeartbeat());
                break;
            case IdleState.ReaderIdle:
                context.CloseAsync();
                break;
            case IdleState.AllIdle:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}