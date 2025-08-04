using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class ExchangeRate
{
    public Currency From { get; init; }
    public Currency To { get; init; }
    public decimal Rate { get; init; }
}
