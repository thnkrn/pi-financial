using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public class FundInfo
{
    public FundInfo(string name, string fundCode, string logo, string amcCode)
    {
        Name = name;
        FundCode = fundCode;
        Logo = logo;
        AmcCode = amcCode;
    }

    public string? InstrumentCategory { get; set; }
    public string Name { get; set; }
    public string FundCode { get; set; }
    public string Logo { get; set; }
    public string AmcCode { get; set; }
    public decimal? Nav { get; set; }
    public required decimal FirstMinBuyAmount { get; set; }
    public required decimal NextMinBuyAmount { get; set; }
    public required decimal MinSellAmount { get; set; }
    public required decimal MinSellUnit { get; set; }
    public required decimal MinBalanceAmount { get; set; }
    public required decimal MinBalanceUnit { get; set; }
    public required DateTime PiBuyCutOffTime { get; set; }
    public required DateTime PiSellCutOffTime { get; set; }
    public FundProjectType? ProjectType { get; set; }
    public string? TaxType { get; set; } // e.g. SSF,LTF

    public bool VerifyCustomerAccess(CustomerAccountDetail customerAccountDetail)
    {
        var requiredHNW = new[] { InvestorClass.HighNetWorth, InvestorClass.UltraHighNetWorth };
        var investorClassesFilters = ProjectType switch
        {
            FundProjectType.NonRetailInvestorsFund => requiredHNW,
            FundProjectType.NonRetailAndHighNetWorthInvestorsFund => requiredHNW,
            FundProjectType.InstitutionalAndSpecialLargeInvestorsFund => new[] { InvestorClass.UltraHighNetWorth },
            FundProjectType.HighNetWorthInvestorsFund => requiredHNW,
            _ => null,
        };

        if (customerAccountDetail.InvestorClass == null) return investorClassesFilters == null;

        return investorClassesFilters == null || investorClassesFilters.Contains((InvestorClass)customerAccountDetail.InvestorClass);
    }
}
