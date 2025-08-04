namespace Pi.FundMarketData.API.Models.Responses;

public class SwitchingFundResponse
{
    public IEnumerable<SwitchFunds> SwitchFunds { get; init; } = Enumerable.Empty<SwitchFunds>();
}

public class SwitchFunds
{
    public string Name { get; init; }
    public decimal Nav { get; init; }
    public string Symbol { get; init; }
    public double? ReturnPercentage { get; init; }
    public string Logo { get; init; }
}

