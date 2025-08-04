using QuickFix;

namespace Pi.GlobalMarketData.Infrastructure.Services.FIX;

public interface IClient
{
    void Setup(string configFile);
    void Start();
    void Shutdown();
    void Send(Message message);
}