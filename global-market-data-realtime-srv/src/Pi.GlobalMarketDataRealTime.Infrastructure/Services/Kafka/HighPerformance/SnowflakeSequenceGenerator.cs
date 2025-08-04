namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public class SnowflakeSequenceGenerator
{
    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;
    private const long MaxMachineId = (1L << MachineIdBits) - 1;
    private const long MaxSequence = (1L << SequenceBits) - 1;

    private static readonly Lazy<SnowflakeSequenceGenerator> InstanceHolder =
        new(() => new SnowflakeSequenceGenerator());

    private static readonly long
        Epoch = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

    private readonly object _lock = new();
    private readonly long _machineId;
    private long _lastTimestamp = -1L;
    private long _sequence;

    private SnowflakeSequenceGenerator()
    {
        _machineId = GenerateMachineId();
        if (_machineId is > MaxMachineId or < 0)
            throw new ArgumentException($"Machine ID should be between 0 and {MaxMachineId}");
    }

    public static SnowflakeSequenceGenerator Instance => InstanceHolder.Value;

    public long GetNextSequence()
    {
        lock (_lock)
        {
            var timestamp = GetCurrentTimestamp();
            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Clock moved backwards. Refusing to generate ID");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                    // Sequence exhausted, wait for next millisecond
                    timestamp = WaitForNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Epoch) << (MachineIdBits + SequenceBits))
                   | (_machineId << SequenceBits)
                   | _sequence;
        }
    }

    private static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static long WaitForNextMillis(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp) timestamp = GetCurrentTimestamp();
        return timestamp;
    }

    private static long GenerateMachineId()
    {
        var hostName = Environment.MachineName;

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOSTNAME")))
            hostName = Environment.GetEnvironmentVariable("HOSTNAME");

        if (string.IsNullOrEmpty(hostName))
            hostName = "default-host";

        return Math.Abs(hostName.GetHashCode()) % (MaxMachineId + 1);
    }
}