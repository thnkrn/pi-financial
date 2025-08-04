using Pi.SetMarketData.Application.Interfaces.Holiday;

namespace Pi.SetMarketData.Application.Queries.Holiday;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}