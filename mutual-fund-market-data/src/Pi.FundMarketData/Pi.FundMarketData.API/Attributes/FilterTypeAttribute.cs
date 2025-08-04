using Pi.FundMarketData.API.Models.Filters;

namespace Pi.FundMarketData.API.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FilterTypeAttribute(FilterType filterType) : Attribute
{
    public FilterType FilterType { get; set; } = filterType;
}
