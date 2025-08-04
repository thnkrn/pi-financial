using QuickFix;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

// ReSharper disable UnusedMemberInSuper.Global
public interface IFixListener : IApplication
{
    bool IsInitialService { get; set; }
    void SendMessage(Message message);
    bool CheckSession();
}