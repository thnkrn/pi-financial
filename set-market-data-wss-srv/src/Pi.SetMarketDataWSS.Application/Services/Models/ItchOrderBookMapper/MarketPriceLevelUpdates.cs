using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

public class MarketByPriceLevelWrapper : ItchMessage
{
    public MarketByPriceLevelWrapper()
    {
        MsgType = ItchMessageType.b;
    }

    public Numeric32 Nanos { get; set; }
    public Numeric32 OrderBookID { get; set; }
    public Numeric8 MaximumLevel { get; set; }
    public Numeric8 NumberOfLevelItems { get; set; }
    public List<PriceLevelUpdate> PriceLevelUpdates { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}

public class PriceLevelUpdate
{
    public Alpha UpdateAction { get; set; }
    public Alpha Side { get; set; }
    public Numeric8 Level { get; set; }
    public Price64 Price { get; set; }
    public Numeric64 Quantity { get; set; }
    public Numeric8 NumberOfDeletes { get; set; }
}