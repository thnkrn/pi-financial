using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.DataServer.Helpers;
using Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.DataServer.Handlers;

internal class Handler(ServerListener listener) : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        switch (message)
        {
            case Debug msg:
                Console.WriteLine("Debug msg");
                _ = listener.OnDebug(msg.Text, context.Channel.Id.AsLongText());
                break;
            case LogoutRequest msg:
                if (msg.Bytes != null)
                    Console.WriteLine($"LogoutRequest msg, Length: {msg.Bytes.Length}");
                _ = listener.OnLogout(context.Channel.Id.AsLongText());
                context.CloseAsync();
                break;
            case UnSequencedData msg:
                Console.WriteLine("UnSequencedData msg");
                listener.OnMessage(msg.Message, context.Channel.Id.AsLongText()).Wait();

                // Start streaming data asynchronously
                _ = StreamDataHelper.StreamDataAsync(context);

                break;
            case SequencedData msg:
                Console.WriteLine("SequencedData msg");
                listener.OnMessage(msg.Message, context.Channel.Id.AsLongText()).Wait();

                // Start streaming data asynchronously
                _ = StreamDataHelper.StreamDataAsync(context);
                break;
        }
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        listener.OnSessionEnd(context.Channel.Id.AsLongText()).Wait();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine("Exception: " + exception);
        context.CloseAsync();
    }
}