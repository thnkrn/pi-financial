namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public class AutoSequenceGenerator
{
    private static readonly Lazy<AutoSequenceGenerator> AutoSequenceGeneratorInstance =
        new(() => new AutoSequenceGenerator());

    private readonly short _instanceId;
    private readonly object _sequenceLock = new();
    private long _lastTicks;

    private AutoSequenceGenerator()
    {
        var hostInfo = Environment.MachineName;

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOSTNAME")))
            hostInfo = Environment.GetEnvironmentVariable("HOSTNAME");

        if (hostInfo != null)
            _instanceId = (short)(Math.Abs(hostInfo.GetHashCode()) % 65536);
    }

    public static AutoSequenceGenerator Instance => AutoSequenceGeneratorInstance.Value;

    public long GetNextSequence()
    {
        lock (_sequenceLock)
        {
            var currentTicks = DateTime.UtcNow.Ticks;
            if (currentTicks <= _lastTicks)
                currentTicks = _lastTicks + 1;

            _lastTicks = currentTicks;

            return (currentTicks << 16) | (ushort)_instanceId;
        }
    }
}