namespace Pi.FundMarketData.DomainModels;

public class Switching
{
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }
    public string SwitchingType { get; init; }
    public int? SwitchSettlementDay { get; init; }
}
