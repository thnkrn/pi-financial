using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

public class MarketDirectoryMessageWrapper : ItchMessage
{
    public MarketDirectoryMessageWrapper()
    {
        MsgType = ItchMessageType.m;
    }

    public Nanos? Nanos { get; set; }
    public MarketCode? MarketCode { get; set; }
    public MarketName? MarketName { get; set; }
    public MarketDescription? MarketDescription { get; set; }
    public Metadata? Metadata { get; set; }

    public new char MsgType
    {
        get => base.MsgType;
        private set => base.MsgType = value;
    }
}