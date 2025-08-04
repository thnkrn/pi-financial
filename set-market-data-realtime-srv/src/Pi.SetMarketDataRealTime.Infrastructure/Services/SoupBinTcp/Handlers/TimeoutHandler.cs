using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

public class TimeoutHandler(ILogger<TimeoutHandler> logger) : ChannelDuplexHandler
{
    private readonly ILogger<TimeoutHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override void UserEventTriggered(IChannelHandlerContext context, object evt)
    {
        if (evt is not IdleStateEvent idleEvent)
        {
            base.UserEventTriggered(context, evt);
            return;
        }

        _logger.LogDebug("Idle event triggered: {IdleState}", idleEvent.State);

        try
        {
            HandleIdleState(context, idleEvent.State);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling idle state: {IdleState}", idleEvent.State);
            context.FireExceptionCaught(ex);
        }
    }

    private void HandleIdleState(IChannelHandlerContext context, IdleState state)
    {
        switch (state)
        {
            case IdleState.WriterIdle:
                HandleWriterIdle(context);
                break;
            case IdleState.ReaderIdle:
                HandleReaderIdle(context);
                break;
            case IdleState.AllIdle:
                HandleAllIdle();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown idle state");
        }
    }

    private void HandleWriterIdle(IChannelHandlerContext context)
    {
        _logger.LogDebug("Writer idle - sending heartbeat");
        context.WriteAndFlushAsync(new ClientHeartbeat());
    }

    private void HandleReaderIdle(IChannelHandlerContext context)
    {
        _logger.LogWarning("Reader idle - closing connection");
        context.CloseAsync();
    }

    private void HandleAllIdle()
    {
        _logger.LogWarning("All idle detected - considering server unresponsive");
        // In an all idle state, both read and write operations have been inactive
        // This could indicate a problem with the server or network connection
        // Depending on the application's requirements, system might want to:
        // 1. Attempt to reconnect
        // 2. Notify a monitoring system
        // 3. Trigger an application-specific recovery process

        // For now, application will just log the event, but system should implement appropriate handling
        _logger.LogError(
            "Server is unresponsive. Consider implementing reconnection logic or notifying administrators.");
    }
}