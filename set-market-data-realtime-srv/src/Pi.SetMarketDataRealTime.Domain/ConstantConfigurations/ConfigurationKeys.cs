namespace Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;

public static class ConfigurationKeys
{
    public const string KafkaBootstrapServers = "KAFKA:BOOTSTRAP_SERVERS";
    public const string KafkaTopic = "KAFKA:TOPIC";
    public const string KafkaBidOfferTopic = "KAFKA:BID_OFFER_TOPIC";
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
    public const string RedisSsl = "REDIS:SSL";
    public const string RedisConnectTimeout = "REDIS:CONNECT_TIMEOUT";
    public const string RedisSyncTimeout = "REDIS:SYNC_TIMEOUT";
    public const string RedisAbortOnConnectFail = "REDIS:ABORT_ON_CONNECT_FAIL";
    public const string RedisConnectRetry = "REDIS:CONNECT_RETRY";
    public const string RedisKeySpace = "REDIS:KEY_SPACE";
    public const string RedisTieBreaker = "REDIS:TIE_BREAKER";

    public const string SignalRBaseUrl = "SIGNALR_BASE_URL";
    public const string SignalRHubGroupName = "SIGNALR_HUB:GROUP_NAME";
    public const string SignalRHubMethodName = "SIGNALR_HUB:METHOD_NAME";

    public const string StreamingHubGroupName = "STREAMING_HUB:GROUP_NAME";
    public const string StreamingHubMethodName = "STREAMING_HUB:METHOD_NAME";
    public const string CorsSettingsAllowedOrigins = "CORS_SETTINGS:ALLOWED_ORIGINS";

    public const string BinLogRegion = "AWS_S3:REGION";
    public const string BinLogBucketName = "AWS_S3:BUCKET_NAME";
    public const string BinLogIsActivated = "AWS_S3:IS_ACTIVATED";

    public const string SettradeRunLocalMode = "SETTRADE_RUN_LOCAL_MODE";
    public const string SettradeRunOnStartup = "SETTRADE_RUN_ON_STARTUP";
    public const string SettradeClientConfigLoginDetails = "SETTRADE_CLIENT_CONFIG:LOGIN_DETAILS";
    public const string SettradeClientConfigLoginDetailsUsername = "SETTRADE_CLIENT_CONFIG:LOGIN_DETAILS:USERNAME";
    public const string SettradeClientConfigLoginDetailsPassword = "SETTRADE_CLIENT_CONFIG:LOGIN_DETAILS:PASSWORD";
    public const string SettradeClientConfigIpAddress = "SETTRADE_CLIENT_CONFIG:IP_ADDRESS";
    public const string SettradeClientConfigPort = "SETTRADE_CLIENT_CONFIG:PORT";
    public const string SettradeClientConfigReconnectDelayMs = "SETTRADE_CLIENT_CONFIG:RECONNECT_DELAY_MS";
    public const string SettradeClientConfigRequestDataAfterLogon = "SETTRADE_CLIENT_CONFIG:REQUEST_DATA_AFTER_LOGON";
    
    public const string SettradeGatewaySettings = "SETTRADE_GATEWAY_SETTINGS";
    public const string SettradeGatewaySettingsServers = "SETTRADE_GATEWAY_SETTINGS:SERVERS";
    public const string SettradeGatewaySettingsReconnectDelayMs = "SETTRADE_GATEWAY_SETTINGS:RECONNECT_DELAY_MS";
    public const string SettradeGatewaySettingsFailOverDelayMs = "SETTRADE_GATEWAY_SETTINGS:FAIL_OVER_DELAY_MS";

    public const string SettradeStockMarketStartCronJob = "SETTRADE_STOCK_MARKET:START_CRON_JOB";
    public const string SettradeStockMarketStopCronJob = "SETTRADE_STOCK_MARKET:STOP_CRON_JOB";

    public const string ServerConfigStreamDataPath = "SERVER_CONFIG:STREAM_DATA_PATH";
    public const string WriteStockMessageToFile = "SERVER_CONFIG:WRITE_STOCK_MESSAGE_TO_FILE";

    public const string SettradeHolidayApiIsActivated = "SETTRADE_HOLIDAY_API:IS_ACTIVATED";
    public const string SettradeHolidayApiBaseUrl = "SETTRADE_HOLIDAY_API:BASE_URL";
    public const string SettradeHolidayApiUseSecureEndpoint = "SETTRADE_HOLIDAY_API:USE_SECURE_ENDPOINT";
    public const string SettradeHolidayApiInternalEndpoint = "SETTRADE_HOLIDAY_API:INTERNAL_ENDPOINT";
    public const string SettradeHolidayApiSecureEndpoint = "SETTRADE_HOLIDAY_API:SECURE_ENDPOINT";
    
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
    public const string Product = "PRODUCT";
}