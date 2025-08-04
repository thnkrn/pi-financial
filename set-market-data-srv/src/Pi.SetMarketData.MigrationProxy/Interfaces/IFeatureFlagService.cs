namespace Pi.SetMarketData.MigrationProxy.Interfaces;

public interface IFeatureFlagService 
{
    bool IsSETWebsocketProxyEnabled();
    bool IsTFEXWebsocketProxyEnabled();
    bool IsGEWebsocketProxyEnabled();
    bool IsGenericHttpProxyEnabled();
    bool IsSETHttpProxyEnabled();
    bool IsTFEXHttpProxyEnabled();
    bool IsGEHttpProxyEnabled();
}