using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class MarketByPriceLevelWrapper : ItchMessage
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookID { get; set; }
    public Numeric8 MaximumLevel { get; set; }
    public Numeric8 NumberOfLevelItems { get; set; }
#pragma warning disable CS8618
    public List<PriceLevelUpdate> PriceLevelUpdates { get; set; }
#pragma warning restore CS8618
}

public class PriceLevelUpdate
{
    public Numeric8 Level { get; set; }
    public Price64 Price { get; set; }
    public Numeric64 Quantity { get; set; }
    public Numeric8 NumberOfDeletes { get; set; }
#pragma warning disable CS8618
    public Alpha UpdateAction { get; set; }
    public Alpha Side { get; set; }
#pragma warning restore CS8618
}