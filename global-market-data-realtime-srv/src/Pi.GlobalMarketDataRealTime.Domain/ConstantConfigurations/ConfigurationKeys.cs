namespace Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;

// ReSharper disable InconsistentNaming
public static class ConfigurationKeys
{
    public const string MongoConnection = "MONGODB_SETTINGS:CONNECTION_STRINGS";
    public const string MongoDatabase = "MONGODB_SETTINGS:DATABASE_NAMES";
    public const string MongoConnectionUserName = "MONGODB_SETTINGS:CONNECTION_USERNAME";
    public const string MongoConnectionPassword = "MONGODB_SETTINGS:CONNECTION_PASSWORD";

    public const string KafkaBootstrapServers = "KAFKA:BOOTSTRAP_SERVERS";
    public const string KafkaTopic = "KAFKA:TOPIC";
    public const string KafkaTradeDataTopic = "KAFKA:TRADE_DATA_TOPIC";
    public const string KafkaDataRecoveryTopic = "KAFKA:DATA_RECOVERY_TOPIC";
    public const string KafkaConsumerGroupId = "KAFKA:CONSUMER_GROUP_ID";
    public const string KafkaSecurityProtocol = "KAFKA:SECURITY_PROTOCOL";
    public const string KafkaSaslMechanism = "KAFKA:SASL_MECHANISM";
    public const string KafkaSaslUsername = "KAFKA:SASL_USERNAME";
    public const string KafkaSaslPassword = "KAFKA:SASL_PASSWORD";
    public const string KafkaSessionTimeoutMs = "KAFKA:SESSION_TIMEOUT_MS";

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
    public const string RedisUseLocalCache = "REDIS:USE_LOCAL_CACHE";
    public const string RedisConnectionPoolSize = "REDIS:CONNECTION_POOL_SIZE";
    public const string RedisSsl = "REDIS:SSL";
    public const string RedisConnectTimeout = "REDIS:CONNECT_TIMEOUT";
    public const string RedisSyncTimeout = "REDIS:SYNC_TIMEOUT";
    public const string RedisAbortOnConnectFail = "REDIS:ABORT_ON_CONNECT_FAIL";
    public const string RedisConnectRetry = "REDIS:CONNECT_RETRY";
    public const string RedisTieBreaker = "REDIS:TIE_BREAKER";
    
    public const string FixConfig = "FIX";
    public const string FixConfigFiles = "FIX:CONFIG_FILES";
    public const string VelexaApi = "VELEXA_API";
    public const string VelexaHttpApiBaseUrl = "VELEXA:API_URL";
    public const string VelexaApiVersion = "VELEXA:API_VERSION";
    public const string VelexaHttpApiJwtSecret = "VELEXA:JWT_SECRET";
    public const string VelexaHttpApiJwtClientId = "VELEXA:JWT_CLIENT_ID";
    public const string VelexaHttpApiJwtAppId = "VELEXA:JWT_APP_ID";
    public const string VelexaMarketSchedule = "VELEXA:MARKET_SCHEDULE_CRON";
    public const string DataRecoveryCacheKey = "REDIS:DATA_RECOVERY_CACHE_KEY";
    public const string InstanceConfigProfile = "INSTANCE_CONFIG_PROFILE";
    public const string RunClient = "RUN_CLIENT";
    
    public const string ProducerConfigsBatchSize = "KAFKA:PRODUCER_CONFIGS:BATCH_SIZE";
    public const string ProducerConfigsLingerMs ="KAFKA:PRODUCER_CONFIGS:LINGER_MS";
    public const string producerConfigsQueueBufferingMaxMessages ="KAFKA:PRODUCER_CONFIGS:QUEUE_BUFFERING_MAX_MESSAGES";
    public const string producerConfigsQueueBufferingMaxKBytes ="KAFKA:PRODUCER_CONFIGS:QUEUE_BUFFERING_MAX_K_BYTES";
    public const string ProducerConfigsMessageSendMaxRetries ="KAFKA:PRODUCER_CONFIGS:MESSAGE_SEND_MAX_RETRIES";
    public const string ProducerConfigsRetryBackoffMs ="KAFKA:PRODUCER_CONFIGS:RETRY_BACKOFF_MS";
    public const string ProducerConfigsSocketSendBufferBytes ="KAFKA:PRODUCER_CONFIGS:SOCKET_SEND_BUFFER_BYTES";
    public const string ProducerConfigsSocketReceiveBufferBytes ="KAFKA:PRODUCER_CONFIGS:SOCKET_RECEIVE_BUFFER_BYTES";
    public const string ProducerConfigsSocketTimeoutMs ="KAFKA:PRODUCER_CONFIGS:SOCKET_TIMEOUT_MS";
    public const string ProducerConfigsMessageTimeoutMs ="KAFKA:PRODUCER_CONFIGS:MESSAGE_TIMEOUT_MS";
    public const string ProducerConfigsRequestTimeoutMs ="KAFKA:PRODUCER_CONFIGS:REQUEST_TIMEOUT_MS";
    public const string ProducerConfigsMaxInFlight ="KAFKA:PRODUCER_CONFIGS:MAX_IN_FLIGHT";
    public const string ProducerConfigsEnableIdempotence ="KAFKA:PRODUCER_CONFIGS:ENABLE_IDEMPOTENCE";
    public const string ProducerConfigsReconnectBackoffMs ="KAFKA:PRODUCER_CONFIGS:RECONNECT_BACKOFF_MS";
    public const string ProducerConfigsReconnectBackoffMaxMs ="KAFKA:PRODUCER_CONFIGS:RECONNECT_BACKOFF_MAX_MS";
    public const string MessagePublisherOptionsBatchSize ="KAFKA:MESSAGE_PUBLISHER_OPTIONS:BATCH_SIZE";
    public const string MessagePublisherOptionsBatchWindowMs ="KAFKA:MESSAGE_PUBLISHER_OPTIONS:BATCH_WINDOW_MS";
    public const string MessagePublisherOptionsChannelCapacity ="KAFKA:MESSAGE_PUBLISHER_OPTIONS:CHANNEL_CAPACITY";
    public const string MessagePublisherOptionsEnableBackpressure ="KAFKA:MESSAGE_PUBLISHER_OPTIONS:ENABLE_BACKPRESSURE";
    public const string MessagePublisherOptionsMaxWaitTimeMs = "KAFKA:MESSAGE_PUBLISHER_OPTIONS:MAX_WAIT_TIME_MS";

    public const string MarketSessionStartTime = "MARKET_SESSION:START_TIME";
    public const string MarketSessionEndTime = "MARKET_SESSION:END_TIME";
}