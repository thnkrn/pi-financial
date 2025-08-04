using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;

public class ServerListener
{
    public async Task OnServerListening()
    {
        Console.WriteLine("OnServerListening");
        await Task.CompletedTask;
    }

    public LoginStatus OnLoginRequest(string username, string password, string requestedSession,
        ulong requestedSequenceNumber, string clientId)
    {
        Console.WriteLine("OnLoginRequest");
        return new LoginStatus(true);
    }

    public async Task OnLogout(string clientId)
    {
        Console.WriteLine("OnLogout");
        await Task.CompletedTask;
    }

    public async Task OnDebug(string message, string clientId)
    {
        Console.WriteLine("OnDebug");
        await Task.CompletedTask;
    }

    public async Task OnMessage(byte[] message, string clientId)
    {
        Console.WriteLine("OnMessage");
        await Task.CompletedTask;
    }

    public async Task OnSessionStart(string sessionId)
    {
        Console.WriteLine("OnSessionStart");
        await Task.CompletedTask;
    }

    public async Task OnSessionEnd(string clientId)
    {
        Console.WriteLine("OnSessionEnd");
        await Task.CompletedTask;
    }

    public async Task OnServerDisconnect()
    {
        Console.WriteLine("OnServerDisconnect");
        await Task.CompletedTask;
    }
}