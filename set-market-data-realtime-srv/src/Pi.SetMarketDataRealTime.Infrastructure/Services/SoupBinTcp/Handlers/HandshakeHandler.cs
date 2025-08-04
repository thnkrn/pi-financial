using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

public class HandshakeHandler(LoginDetails loginDetails) : ChannelHandlerAdapter
{
    public override void ChannelActive(IChannelHandlerContext context)
    {
        if (loginDetails == null
            || string.IsNullOrEmpty(loginDetails.UserName)
            || string.IsNullOrEmpty(loginDetails.Password))
            throw new ArgumentException("Login credentials cannot be null or empty!");

        context.WriteAndFlushAsync(new LoginRequest(loginDetails.UserName, loginDetails.Password,
            loginDetails.RequestedSession, loginDetails.RequestedSequenceNumber));
    }
}