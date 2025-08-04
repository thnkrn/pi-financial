using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

public enum InvestorClass
{
    None, // For support old InvestorClass
    UltraHighNetWorth,
    HighNetWorth,
    Retail,
    Institutional
}
