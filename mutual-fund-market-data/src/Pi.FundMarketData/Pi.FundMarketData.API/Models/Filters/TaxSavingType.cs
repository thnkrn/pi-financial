using System.ComponentModel;

namespace Pi.FundMarketData.API.Models.Filters;

public enum TaxSavingType
{
    [Description("RMF")] Rmf,
    [Description("Thai ESG")] TEsg,
    [Description("Thai ESGX")] TEsgx,
}
