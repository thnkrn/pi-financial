using DotNetty.Transport.Channels;
using Pi.SetMarketData.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketData.Infrastructure.Services.SoupBinTcp.Handlers;

internal class Handler(LoginDetails loginDetails, IClientListener clientListener) : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        switch (message)
        {
            case Debug msg:
                // Console.WriteLine(msg);
                clientListener.OnDebug(msg.Text);
                break;
            case UnSequencedData msg:
                // Console.WriteLine(msg);
                clientListener.OnMessage(msg.Message);
                break;
            case SequencedData msg:
                clientListener.OnMessage(msg.Message);
                break;
            case LoginAccepted msg:
                // Console.WriteLine(msg);
                clientListener.OnLoginAccept(msg.Session, msg.SequenceNumber);
                break;
        }
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        clientListener.OnConnect();

        if (string.IsNullOrEmpty(loginDetails.UserName) || string.IsNullOrEmpty(loginDetails.Password))
        {
            throw new ArgumentException("The user name or password is incorrect!");
        }

        context.WriteAndFlushAsync(new LoginRequest(loginDetails.UserName, loginDetails.Password,
            loginDetails.RequestedSession, loginDetails.RequestedSequenceNumber));
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        clientListener.OnDisconnect();
    }
    
    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine($"Exception caught: {exception}");
        context.CloseAsync();
    }
}