namespace Pi.FundMarketData.API.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FilterOrderAttribute(int order) : Attribute
{
    public int Order { get; set; } = order;
}
