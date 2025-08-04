namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.SoupBinTcp;

public interface IClientListener
{
    Task OnConnect();
    Task OnMessage(byte[] message);
    Task OnDebug(string message);
    Task OnLoginAccept(string session, ulong sequenceNumber);
    Task OnLoginReject();
    Task OnDisconnect();
}