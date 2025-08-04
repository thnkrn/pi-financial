using Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;

namespace Pi.SetMarketDataRealTime.Application.Queries.Holiday;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}