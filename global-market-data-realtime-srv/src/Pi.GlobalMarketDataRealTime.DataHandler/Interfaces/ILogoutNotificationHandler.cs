using QuickFix;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;

public interface ILogoutNotificationHandler
{
    Task NotifyLogout(SessionID sessionId);
}