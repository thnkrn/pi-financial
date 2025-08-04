using System.Net.Sockets;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

public class Handler : ChannelHandlerAdapter
{
    private readonly IClientListener _clientListener;
    private readonly ILogger<Handler> _logger;
    private readonly LoginDetails _loginDetails;
    private readonly Action<bool> _onUnexpectedDisconnection;
    private volatile bool _isUnexpectedDisconnection;

    /// <summary>
    ///     Handler
    /// </summary>
    /// <param name="loginDetails"></param>
    /// <param name="clientListener"></param>
    /// <param name="logger"></param>
    /// <param name="onUnexpectedDisconnection"></param>
    public Handler(LoginDetails loginDetails, IClientListener clientListener, ILogger<Handler> logger,
        Action<bool> onUnexpectedDisconnection)
    {
        _loginDetails = loginDetails;
        _clientListener = clientListener;
        _logger = logger;
        _onUnexpectedDisconnection = onUnexpectedDisconnection;
        _isUnexpectedDisconnection = true;
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        switch (message)
        {
            case Debug msg:
                _clientListener.OnDebug(msg.Text);
                break;
            case UnSequencedData msg:
                _clientListener.OnMessage(msg.Message);
                break;
            case SequencedData msg:
                _clientListener.OnMessage(msg.Message);
                break;
            case LoginAccepted msg:
                _clientListener.OnLoginAccept(msg.Session, msg.SequenceNumber);
                break;
            case LoginRejected msg:
                _isUnexpectedDisconnection = false;
                _clientListener.OnLoginReject(msg.RejectReasonCode);
                break;
            default:
                _logger.LogDebug("Unhandled message type: {MessageType}", message.GetType().Name);
                break;
        }
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        _logger.LogDebug("Channel activated");
        _clientListener.OnConnect();

        try
        {
            if (string.IsNullOrEmpty(_loginDetails.UserName) || string.IsNullOrEmpty(_loginDetails.Password))
                throw new ArgumentException("The user name or password is incorrect!");

            context.WriteAndFlushAsync(new LoginRequest(_loginDetails.UserName, _loginDetails.Password,
                _loginDetails.RequestedSession, _loginDetails.RequestedSequenceNumber));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during channel activation");
            _onUnexpectedDisconnection(_isUnexpectedDisconnection);

            if (ShouldCloseChannelForException(ex))
            {
                _logger.LogWarning("Closing channel due to exception during activation");
                context.CloseAsync();
            }
        }
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        _logger.LogDebug("Channel deactivated");
        _onUnexpectedDisconnection(_isUnexpectedDisconnection);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        _logger.LogError(exception, "Exception caught in channel");
        _onUnexpectedDisconnection(_isUnexpectedDisconnection);

        if (ShouldCloseChannelForException(exception)) context.CloseAsync();
    }

    private static bool ShouldCloseChannelForException(Exception ex)
    {
        return ex is ArgumentException or SocketException or IOException;
    }
}