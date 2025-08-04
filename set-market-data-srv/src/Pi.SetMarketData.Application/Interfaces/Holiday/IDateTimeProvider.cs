namespace Pi.SetMarketData.Application.Interfaces.Holiday;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}