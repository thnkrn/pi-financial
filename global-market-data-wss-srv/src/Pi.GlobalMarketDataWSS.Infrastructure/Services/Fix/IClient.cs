using QuickFix;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Fix;

public interface IClient
{
    void Setup(string configFile);
    void Start();
    void Shutdown();
    void Send(Message message);
}