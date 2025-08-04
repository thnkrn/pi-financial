namespace Pi.MarketData.Application.Interfaces;

public interface IFeatureFlagService
{
    bool IsSetWebsocketProxyEnabled();
    bool IsTfexWebsocketProxyEnabled();
    bool IsGeWebsocketProxyEnabled();
    bool IsGenericHttpProxyEnabled();
    bool IsSetHttpProxyEnabled();
    bool IsTfexHttpProxyEnabled();
    bool IsGeHttpProxyEnabled();
    bool IsUserFavoriteProxyEnabled();
}