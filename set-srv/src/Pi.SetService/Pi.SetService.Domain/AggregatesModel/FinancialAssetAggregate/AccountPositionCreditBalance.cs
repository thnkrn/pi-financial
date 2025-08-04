namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public record AccountPositionCreditBalance : AccountPosition
{
    public AccountPositionCreditBalance(string secSymbol, Ttf ttf) : base(secSymbol, ttf)
    {
    }

    public decimal? LastSale { get; set; }
    public decimal? MktValue { get; set; }
    public decimal? MR { get; set; }
    public string? Grade { get; set; }
    public decimal? AvgCost { get; set; }
    public decimal? TodayMargin { get; set; }
    public decimal? StartClose { get; set; }
    public bool UpdateFlag { get; set; }
    public bool DelFlag { get; set; }
}
