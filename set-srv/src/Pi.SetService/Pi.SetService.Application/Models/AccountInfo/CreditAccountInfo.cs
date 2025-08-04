using Pi.SetService.Domain.AggregatesModel.AccountAggregate;

namespace Pi.SetService.Application.Models.AccountInfo;

public record CreditAccountInfo : AccountInfo
{
    private const decimal Percentage = 100;

    public required decimal Liability { get; init; }
    public required decimal Asset { get; init; }
    public required decimal Equity { get; init; }
    public required decimal MarginRequired { get; init; }
    public required decimal ExcessEquity { get; init; }
    public required decimal CallForce { get; init; }
    public required decimal CallMargin { get; init; }
    public Pc? Pc { get; init; }
    public decimal? Lmv { get; init; }
    public decimal? Collat { get; init; }
    public decimal? Debt { get; init; }
    public decimal? Smv { get; init; }
    public decimal? BuyMR { get; init; }
    public decimal? SellMR { get; init; }
    public decimal? Pp { get; init; }
    public decimal? BrkCallLMV { get; init; }
    public decimal? BrkCallSMV { get; init; }
    public decimal? BrkSellLMV { get; init; }
    public decimal? BrkSellSMV { get; init; }
    public decimal? BuyUnmatch { get; init; }
    public decimal? SellUnmatch { get; init; }
    public decimal? Ar { get; init; }
    public decimal? Ap { get; init; }
    public decimal? ArT1 { get; init; }
    public decimal? ApT1 { get; init; }
    public decimal? ArT2 { get; init; }
    public decimal? ApT2 { get; init; }
    public decimal? Withdrawal { get; init; }
    public decimal? LmvHaircut { get; init; }
    public decimal? EquityHaircut { get; init; }
    public decimal? EE1 { get; init; }
    public decimal? EE50 { get; init; }
    public decimal? EE60 { get; init; }
    public decimal? EE70 { get; init; }
    public decimal? Eemtm { get; init; }
    public decimal? EemtM50 { get; init; }
    public decimal? EemtM60 { get; init; }
    public decimal? EemtM70 { get; init; }
    public string? Action { get; init; }
    public bool DelFlag { get; init; }
    public bool UpdateFlag { get; init; }

    public decimal MmPercentage
    {
        get
        {
            if (Asset == 0) return 0m;

            var equityAssetRatio = decimal.Divide(Equity, Asset);
            return equityAssetRatio < 1 ? decimal.Multiply(equityAssetRatio, Percentage) : Percentage;
        }
    }
}