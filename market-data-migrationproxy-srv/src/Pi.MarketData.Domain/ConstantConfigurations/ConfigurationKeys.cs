namespace Pi.MarketData.Domain.ConstantConfigurations;

public static class ConfigurationKeys
{
    // Websocket
    public const string SetSignalRHubUrl = "SIGNAL_R_HUB:SET_BASE_URL";
    public const string GeSignalRHubUrl = "SIGNAL_R_HUB:GE_BASE_URL";
    public const string WebSocketUrl = "WEBSOCKET:BASE_URL";

    // HTTP Client
    public const string SetBaseUrl = "HTTP_CLIENT:SET";
    public const string GeBaseUrl = "HTTP_CLIENT:GE";
    public const string SiriusBaseUrl = "HTTP_CLIENT:SIRIUS";
    public const string CommonBaseUrl = "HTTP_CLIENT:COMMON";
    public const string SearchBaseUrl = "HTTP_CLIENT:SEARCH";
    public const string SearchV2BaseUrl = "HTTP_CLIENT:SEARCHV2";

    // GrowthBook
    public const string GrowthBookHost = "GrowthBook:Host";
    public const string GrowthBookClientKey = "GrowthBook:ClientKey";
    public const string GrowthBookApiHost = "GrowthBook:ApiHost";

    public const string GrowthBookSetWebsocketProxyKey = "GrowthBook:FEATURE_KEY:SET_WEBSOCKET_PROXY";
    public const string GrowthBookTfexWebsocketProxyKey = "GrowthBook:FEATURE_KEY:TFEX_WEBSOCKET_PROXY";
    public const string GrowthBookGeWebsocketProxyKey = "GrowthBook:FEATURE_KEY:GE_WEBSOCKET_PROXY";

    public const string GrowthBookGenericHttpProxyKey = "GrowthBook:FEATURE_KEY:GENERIC_HTTP_API_PROXY";
    public const string GrowthBookSetHttpProxyKey = "GrowthBook:FEATURE_KEY:SET_HTTP_API_PROXY";
    public const string GrowthBookTfexHttpProxyKey = "GrowthBook:FEATURE_KEY:TFEX_HTTP_API_PROXY";
    public const string GrowthBookGeHttpProxyKey = "GrowthBook:FEATURE_KEY:GE_HTTP_API_PROXY";
    public const string GrowthBookUserFavoriteHttpProxyKey = "GrowthBook:FEATURE_KEY:USER_FAVORITE_API_PROXY";

    // GrowthBook ByPass
    public const string GrowthBookByPassActivated = "GrowthBook:GROWTHBOOK_BYPASS:ACTIVATED";

    public const string GrowthBookByPassSetWebsocket = "GrowthBook:GROWTHBOOK_BYPASS:SET_WEBSOCKET_PROXY";
    public const string GrowthBookByPassTfexWebsocket = "GrowthBook:GROWTHBOOK_BYPASS:TFEX_WEBSOCKET_PROXY";
    public const string GrowthBookByPassGeWebsocket = "GrowthBook:GROWTHBOOK_BYPASS:GE_WEBSOCKET_PROXY";

    public const string GrowthBookByPassGenericHttp = "GrowthBook:GROWTHBOOK_BYPASS:GENERIC_HTTP_API_PROXY";
    public const string GrowthBookByPassSetHttp = "GrowthBook:GROWTHBOOK_BYPASS:SET_HTTP_API_PROXY";
    public const string GrowthBookByPassTfexHttp = "GrowthBook:GROWTHBOOK_BYPASS:TFEX_HTTP_API_PROXY";
    public const string GrowthBookByPassGeHttp = "GrowthBook:GROWTHBOOK_BYPASS:GE_HTTP_API_PROXY";
    public const string GrowthBookByPassUserFavoriteHttp = "GrowthBook:GROWTHBOOK_BYPASS:USER_FAVORITE_API_PROXY";

    // MongoDB
    public const string MongoConnection = "MONGODB_SETTINGS:CONNECTION_STRINGS";
    public const string MongoConnectionUserName = "MONGODB_SETTINGS:CONNECTION_USERNAME";
    public const string MongoConnectionPassword = "MONGODB_SETTINGS:CONNECTION_PASSWORD";
    public const string MongoDatabase = "MONGODB_SETTINGS:DATABASE_NAMES";
}
