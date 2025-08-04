namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public abstract class ItchMessage
{
    public char MsgType { get; protected set; }
}