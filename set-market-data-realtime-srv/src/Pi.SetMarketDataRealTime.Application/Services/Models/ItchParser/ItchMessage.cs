using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public abstract class ItchMessage
{
    public char MsgType { get; protected set; }
    public ItchMessageMetadata? Metadata { get; set; }
    public virtual Numeric32 Nanos { get; } = new([0, 0, 0, 0]);
}