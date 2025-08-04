using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class EquilibriumPriceMessageWrapper : ItchMessage
{
    public EquilibriumPriceMessageWrapper()
    {
        MsgType = ItchMessageType.Z;
    }

    public Nanos? Nanos { get; set; }
    public OrderBookId? OrderBookId { get; set; }
    public BidQuantity? BidQuantity { get; set; }
    public AskQuantity? AskQuantity { get; set; }
    public EquilibriumPrice? EquilibriumPrice { get; set; }
    public NumberOfDecimals? NumberOfDecimals { get; set; }
    public Metadata? Metadata { get; set; }
}