#nullable enable
using System.ComponentModel;
using Pi.FundMarketData.API.Attributes;
using Pi.FundMarketData.API.Models.Filters;

namespace Pi.FundMarketData.API.Models.Requests;

public interface IBasketFilterRequest;

public class BasketFiltersRequest : IBasketFilterRequest
{
    [FilterOrder(1)]
    [FilterType(FilterType.Primary)]
    [Description("Fund Type")]
    public FundType[]? FundTypes { get; set; }

    [FilterOrder(2)]
    [FilterType(FilterType.Secondary)]
    [Description("Tax Saving")]
    public TaxSavingType[]? TaxSavingTypes { get; set; }

    [FilterOrder(3)]
    [FilterType(FilterType.Secondary)]
    [Description("Fund House")]
    public AmcCode[]? AmcCodes { get; set; }

    [FilterOrder(4)]
    [FilterType(FilterType.Secondary)]
    [Description("Risk Level")]
    public RiskLevel[]? RiskLevels { get; set; }

    [FilterOrder(5)]
    [FilterType(FilterType.Secondary)]
    [Description("Dividend")]
    public Dividend? Dividend { get; set; }
}

// BasketFiltersRequestV2 is temporary used by galaxy project,
// once dividend's yaml issue is fixed this will be removed and migrated back to OG BasketFiltersRequest
public class BasketFiltersRequestV2 : IBasketFilterRequest
{
    [FilterOrder(1)]
    [FilterType(FilterType.Primary)]
    [Description("Fund Type")]
    public FundType[]? FundTypes { get; set; }

    [FilterOrder(2)]
    [FilterType(FilterType.Secondary)]
    [Description("Tax Saving")]
    public TaxSavingType[]? TaxSavingTypes { get; set; }

    [FilterOrder(3)]
    [FilterType(FilterType.Secondary)]
    [Description("Fund House")]
    public AmcCode[]? AmcCodes { get; set; }

    [FilterOrder(4)]
    [FilterType(FilterType.Secondary)]
    [Description("Risk Level")]
    public RiskLevel[]? RiskLevels { get; set; }

    [FilterOrder(5)]
    [FilterType(FilterType.Secondary)]
    [Description("Dividend")]
    public DividendV2? Dividend { get; set; }
}
