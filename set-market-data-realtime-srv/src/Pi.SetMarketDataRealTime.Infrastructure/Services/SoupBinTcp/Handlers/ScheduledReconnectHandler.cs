using DotNetty.Transport.Channels;
using NCrontab;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

public class ScheduledReconnectHandler : ChannelHandlerAdapter
{
    private readonly IClient _client;

    private readonly string _startCronExpression;
    private readonly string _stopCronExpression;
    private IChannelHandlerContext? _context;

    /// <summary>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="startCronExpression"></param>
    /// <param name="stopCronExpression"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ScheduledReconnectHandler(IClient client, string startCronExpression, string stopCronExpression)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _startCronExpression = startCronExpression ?? throw new ArgumentNullException(nameof(startCronExpression));
        _stopCronExpression = stopCronExpression ?? throw new ArgumentNullException(nameof(stopCronExpression));
    }

    public override void HandlerAdded(IChannelHandlerContext context)
    {
        base.HandlerAdded(context);
        _context = context;
        ScheduleNextExecution();
    }

    private void ScheduleNextExecution()
    {
        var now = DateTime.Now;
        var nextStart = CrontabSchedule.Parse(_startCronExpression).GetNextOccurrence(now);
        var nextStop = CrontabSchedule.Parse(_stopCronExpression).GetNextOccurrence(now);
        var nextExecution = nextStart < nextStop ? (nextStart, true) : (nextStop, false);

        _context?.Executor.Schedule(() => ExecuteScheduledAction(nextExecution.Item2).Wait(),
            nextExecution.Item1 - now);
    }

    private async Task ExecuteScheduledAction(bool isStart)
    {
        if (isStart)
        {
            if (_client.State != ClientState.Connected) await _client.StartAsync();
        }
        else
        {
            await _client.LogoutAsync();
        }

        ScheduleNextExecution();
    }
}