using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.DataServer.Helpers;
using Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.DataServer.Handlers;

internal class HandshakeHandler(ServerListener listener) : ChannelHandlerAdapter
{
    private readonly IConfiguration _configuration = ConfigurationHelper.GetConfiguration();

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is LoginRequest msg)
        {
            var result = listener.OnLoginRequest(msg.Username,
                msg.Password,
                msg.RequestedSession,
                msg.RequestedSequenceNumber, context.Channel.Id.AsLongText());

            if (result.Success)
            {
                context.Channel.Pipeline.Remove(this);
                context.Channel.Pipeline.Remove("LoginRequestFilter");
                context.Channel.Pipeline.AddLast(new Handler(listener));
                context.WriteAsync(new LoginAccepted(msg.RequestedSession, msg.RequestedSequenceNumber));

                var autoStream = _configuration.GetValue("ServerConfig:AutoStream", false);
                if (autoStream)
                {
                    // Start streaming data asynchronously
                    Task.Delay(3000).Wait();
                    _ = StreamDataHelper.StreamDataAsync(context);
                }
            }
            else
            {
                var reason = result.RejectionReason switch
                {
                    RejectionReason.NotAuthorised => 'A',
                    RejectionReason.SessionNotAvailable => 'S',
                    _ => 'A'
                };

                Console.WriteLine($"Login rejected for user {msg.Username}. Reason: {result.RejectionReason}");

                try
                {
                    context.WriteAndFlushAsync(new LoginRejected(reason));
                    Console.WriteLine("LoginRejected message sent successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending LoginRejected: {ex.Message}");
                }
                finally
                {
                    context.CloseAsync();
                }
            }
        }
        else
        {
            Console.WriteLine(message);
        }
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        _ = listener.OnSessionStart(context.Channel.Id.AsLongText());
    }

    public override void ChannelReadComplete(IChannelHandlerContext context)
    {
        context.Flush();
    }
}