namespace Pi.SetMarketDataWSS.Domain.Entities;

public class Filter
{
    public int FilterId { get; set; }
    public string? FilterName { get; set; }
    public bool IsHighLight { get; set; }
    public bool IsDefault { get; set; }
    public int Order { get; set; }
    public string? FilterCategory { get; set; }
    public string? FilterType { get; set; }
    public bool SupportSecondaryFilter { get; set; }
}