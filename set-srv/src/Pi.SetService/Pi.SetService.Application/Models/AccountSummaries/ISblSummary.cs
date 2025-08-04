namespace Pi.SetService.Application.Models.AccountSummaries;

public interface ISblSummary
{
    decimal LongMarketValue { get; }
    decimal ShortMarketValue { get; }
    decimal LongCostValue { get; }
    decimal ShortCostValue { get; }
}
