using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.Models;

public class ExchangeRateData
{
    public Currency From { get; init; }
    public Currency To { get; init; }
    public decimal Rate { get; init; }
    public DateTime? ValidUntil { get; init; }
}
