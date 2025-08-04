using QuickFix;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Fix;

public interface IFixListener : IApplication
{
    void SendMessage(Message message);
}