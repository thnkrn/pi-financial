using DotNetty.Transport.Channels;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.SoupBinTcp.Handlers;

public class HandshakeHandler(LoginDetails loginDetails, IClientListener clientListener) : ChannelHandlerAdapter
{
    private readonly IClientListener _clientListener = clientListener;

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