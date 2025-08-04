namespace Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}