namespace Pi.SetMarketData.MigrationProxy.ConstantConfigurations;

public static class ConfigurationKeys
{
    public const string SETSignalRHubURL = "SIGNAL_R_HUB:SET_BASE_URL";
    public const string GESignalRHubURL = "SIGNAL_R_HUB:GE_BASE_URL";
    public const string WebSocketURL = "WEBSOCKET:BASE_URL";
    public const string SETBaseURL = "HTTP_CLIENT:SET";
    public const string GEBaseURL = "HTTP_CLIENT:GE";
    public const string SiriusBaseURL = "HTTP_CLIENT:SIRIUS";
    public const string CommonBaseURL = "HTTP_CLIENT:COMMON";
    public const string GrowthBookBaseUrl = "GrowthBook:BASE_URL";
    public const string GrowthBookClientKey = "GrowthBook:CLIENT_KEY";
    public const string GrowthBookSetWebsocketProxyKey = "GrowthBook:FEATURE_KEY:SET_WEBSOCKET_PROXY";
    public const string GrowthBookTFEXWebsocketProxyKey = "GrowthBook:FEATURE_KEY:TFEX_WEBSOCKET_PROXY";
    public const string GrowthBookGEWebsocketProxyKey = "GrowthBook:FEATURE_KEY:GE_WEBSOCKET_PROXY";
    public const string GrowthBookGenericHttpProxyKey = "GrowthBook:FEATURE_KEY:GENERIC_HTTP_API_PROXY";
    public const string GrowthBookSetHttpProxyKey = "GrowthBook:FEATURE_KEY:SET_HTTP_API_PROXY";
    public const string GrowthBookTFEXHttpProxyKey = "GrowthBook:FEATURE_KEY:TFEX_HTTP_API_PROXY";
    public const string GrowthBookGEHttpProxyKey = "GrowthBook:FEATURE_KEY:GE_HTTP_API_PROXY";
}