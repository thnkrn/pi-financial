using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.DataServer.Handlers;

internal class TimeoutHandler(ServerListener listener) : ChannelDuplexHandler
{
    public override void UserEventTriggered(IChannelHandlerContext context, object evt)
    {
        if (evt is IdleStateEvent e)
            switch (e.State)
            {
                case IdleState.WriterIdle:
                    context.WriteAndFlushAsync(new ServerHeartbeat());
                    break;
                case IdleState.ReaderIdle:
                    context.CloseAsync();
                    _ = listener.OnSessionEnd(context.Channel.Id.AsLongText());
                    break;
                case IdleState.AllIdle:
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}