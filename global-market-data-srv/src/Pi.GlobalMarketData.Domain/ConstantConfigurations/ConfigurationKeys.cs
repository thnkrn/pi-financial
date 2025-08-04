namespace Pi.GlobalMarketData.Domain.ConstantConfigurations;

public static class ConfigurationKeys
{
    public const string KafkaBootstrapServers = "KAFKA:BOOTSTRAP_SERVERS";
    public const string KafkaTopic = "KAFKA:TOPIC";
    public const string KafkaConsumerGroupId = "KAFKA:CONSUMER_GROUP_ID";
    public const string KafkaSecurityProtocol = "KAFKA:SECURITY_PROTOCOL";
    public const string KafkaSaslMechanism = "KAFKA:SASL_MECHANISM";
    public const string KafkaSaslUsername = "KAFKA:SASL_USERNAME";
    public const string KafkaSaslPassword = "KAFKA:SASL_PASSWORD";
    public const string KafkaMigrationJobTopicName = "KAFKA:MIGRATION_JOB_TOPIC_NAME";
    public const string KafkaMigrationDataTopicName = "KAFKA:MIGRATION_DATA_TOPIC_NAME";

    public const string RedisConnectionString = "REDIS:CONNECTION_STRING";
    public const string RedisChannel = "REDIS:CHANNEL";
    public const string RedisEnabled = "REDIS:ENABLED";
    public const string RedisHost = "REDIS:HOST";
    public const string RedisPort = "REDIS:PORT";
    public const string RedisUser = "REDIS:USERNAME";
    public const string RedisPassword = "REDIS:PASSWORD";
    public const string RedisDatabase = "REDIS:DATABASE";
    public const string RedisClientName = "REDIS:CLIENT_NAME";
    public const string RedisKeyspace = "REDIS:KEY_SPACE";
    public const string RedisSsl = "REDIS:SSL";
    public const string RedisConnectTimeout = "REDIS:CONNECT_TIMEOUT";
    public const string RedisSyncTimeout = "REDIS:SYNC_TIMEOUT";
    public const string RedisAbortOnConnectFail = "REDIS:ABORT_ON_CONNECT_FAIL";
    public const string RedisConnectRetry = "REDIS:CONNECT_RETRY";
    public const string RedisTieBreaker = "REDIS:TIE_BREAKER";
    public const string RedisConnectionPoolSize = "REDIS:CONNECTION_POOL_SIZE";

    public const string RedisKeepAlive = "REDIS:KEEP_ALIVE";

    public const string SignalRBaseUrl = "SIGNALR_BASE_URL";
    public const string SignalRHubGroupName = "SIGNALR_HUB:GROUP_NAME";
    public const string SignalRHubMethodName = "SIGNALR_HUB:METHOD_NAME";

    public const string StreamingHubGroupName = "STREAMING_HUB:GROUP_NAME";
    public const string StreamingHubMethodName = "STREAMING_HUB:METHOD_NAME";

    public const string CorsSettingsAllowedOrigins = "CORS_SETTINGS:ALLOWED_ORIGINS";

    public const string TimescaleConnection = "CONNECTION_STRING:TIMESCALE_DB";
    public const string TimescalePoolSize = "CONNECTION_STRING:TIMESCALE_POOL_SIZE";

    public const string MongoConnection = "MONGODB_SETTINGS:CONNECTION_STRINGS";
    public const string MongoConnectionUserName = "MONGODB_SETTINGS:CONNECTION_USERNAME";
    public const string MongoConnectionPassword = "MONGODB_SETTINGS:CONNECTION_PASSWORD";

    public const string MongoDatabase = "MONGODB_SETTINGS:DATABASE_NAMES";

    public const string MorningStarBaseUrl = "MORNING_STAR:HOST";
    public const string MorningStarEmail = "MORNING_STAR:EMAIL";
    public const string MorningStarPassword = "MORNING_STAR:PASSWORD";
    public const string MorningStarCrontab = "MORNING_STAR:CRONTAB";
    public const string MorningStarCenterBaseUrl = "MORNING_STAR_CENTER:BASE_URL";
    public const string MorningStarCenterAccountCode = "MORNING_STAR_CENTER:ACCOUNT_CODE";
    public const string MorningStarCenterPassword = "MORNING_STAR_CENTER:PASSWORD";

    public const string VelexaHttpApiBaseUrl = "VELEXA:API_URL";
    public const string VelexaApiVersion = "VELEXA:API_VERSION";
    public const string VelexaHttpApiJwtSecret = "VELEXA:JWT_SECRET";
    public const string VelexaHttpApiJwtClientId = "VELEXA:JWT_CLIENT_ID";
    public const string VelexaHttpApiJwtApptId = "VELEXA:JWT_APP_ID";
    public const string VelexaTokenExpireInSec = "VELEXA:JWT_TOKEN_EXPIRE_IN_SECOND";
    public const string VelexaLiveToken = "VELEXA:JWT_LIVE_TOKEN";
    public const string LogoBaseUrl = "LOGO_BASE_URL";

    public const string CsvEnabled = "CSV:ENABLED";
    public const string CsvDirectory = "CSV:DIRECTORY";
    public const string PreCacheEnable = "PRE_CACHE:ENABLE";
    public const string PreCacheRankingEnable = "PRE_CACHE:RANKING_ENABLE";

    // ENTITIES_CACHE
    public const string EntityCacheOption1 = "ENTITIES_CACHE:OPTION_1";
    public const string EntityCacheOption2 = "ENTITIES_CACHE:OPTION_2";
    public const string EntityCacheOption3 = "ENTITIES_CACHE:OPTION_3";
    public const string EntityCacheOption4 = "ENTITIES_CACHE:OPTION_4";
    
    public const string MarketSessionStartTime = "MARKET_SESSION:START_TIME";
    public const string MarketSessionEndTime = "MARKET_SESSION:END_TIME";
}
