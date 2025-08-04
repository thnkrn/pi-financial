using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;

public interface IClientListener : IDisposable
{
    string LogPrefix { get; set; }
    Task OnConnect();
    Task OnMessage(byte[] message);
    Task OnDebug(string message);
    Task OnLoginAccept(string session, ulong sequenceNumber);
    Task OnLoginReject(char rejectReasonCode);
    Task OnDisconnect(bool isUnexpectedDisconnection);
    Task<ItchMessage?> ReceiveMessage(CancellationToken cancellationToken);
}