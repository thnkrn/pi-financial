namespace Pi.SetMarketDataWSS.Domain.ConstantConfigurations;

public static class ConfigurationKeys
{
    public const string KafkaBootstrapServers = "KAFKA:BOOTSTRAP_SERVERS";
    public const string KafkaTopic = "KAFKA:TOPIC";
    public const string KafkaConsumerGroupId = "KAFKA:CONSUMER_GROUP_ID";
    public const string KafkaSecurityProtocol = "KAFKA:SECURITY_PROTOCOL";
    public const string KafkaSaslMechanism = "KAFKA:SASL_MECHANISM";
    public const string KafkaSaslUsername = "KAFKA:SASL_USERNAME";
    public const string KafkaSaslPassword = "KAFKA:SASL_PASSWORD";

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
    public const string RedisUseLocalCache = "REDIS:USE_LOCAL_CACHE";
    public const string RedisConnectionPoolSize = "REDIS:CONNECTION_POOL_SIZE";
    public const string RedisConnectTimeout = "REDIS:CONNECT_TIMEOUT";
    public const string RedisSyncTimeout = "REDIS:SYNC_TIMEOUT";
    public const string RedisAbortOnConnectFail = "REDIS:ABORT_ON_CONNECT_FAIL";
    public const string RedisConnectRetry = "REDIS:CONNECT_RETRY";
    public const string RedisTieBreaker = "REDIS:TIE_BREAKER";

    public const string SignalRBaseUrl = "SIGNALR_BASE_URL";
    public const string SignalRHubGroupName = "SIGNALR_HUB:GROUP_NAME";
    public const string SignalRHubMethodName = "SIGNALR_HUB:METHOD_NAME";

    public const string RedisStreamingChannel = "REDIS:STREAMING_CHANNEL";
    public const string StreamingHubGroupName = "STREAMING_HUB:GROUP_NAME";
    public const string StreamingHubMethodName = "STREAMING_HUB:METHOD_NAME";

    public const string CorsSettingsAllowedOrigins = "CORS_SETTINGS:ALLOWED_ORIGINS";

    public const string KafkaSubscriptionServiceMaxRetryAttempts = "KAFKA_SUBSCRIPTION_SERVICE:MAX_RETRY_ATTEMPTS";
    public const string KafkaSubscriptionServiceMaxRetryDelayMs = "KAFKA_SUBSCRIPTION_SERVICE:MAX_RETRY_DELAY_MS";

    public const string KafkaSubscriptionServiceInitialRetryDelayMs =
        "KAFKA_SUBSCRIPTION_SERVICE:INITIAL_RETRY_DELAY_MS";

    public const string KafkaSubscriptionServiceSaveStreamingBody = "KAFKA_SUBSCRIPTION_SERVICE:SAVE_STREAMING_BODY";
    public const string ServerName = "SERVER_NAME:SOURCE";

    public const string MongoConnection = "MONGODB_SETTINGS:CONNECTION_STRINGS";
    public const string MongoConnectionUserName = "MONGODB_SETTINGS:CONNECTION_USERNAME";
    public const string MongoConnectionPassword = "MONGODB_SETTINGS:CONNECTION_PASSWORD";
    public const string MongoDatabase = "MONGODB_SETTINGS:DATABASE_NAMES";
    
    public const string ConsumeProduct = "CONSUME_PRODUCT";
}
