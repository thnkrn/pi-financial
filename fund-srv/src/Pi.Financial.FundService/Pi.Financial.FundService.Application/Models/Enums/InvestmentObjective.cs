using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum InvestmentObjective
{
    [Description("Investment")]
    Investment,
    [Description("RetirementInvestment")]
    RetirementInvestment,
    [Description("ForTaxBenefits")]
    ForTaxBenefits,
    [Description("PleaseSpecify")]
    PleaseSpecify,
}
