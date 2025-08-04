using System.Text;
using Confluent.Kafka;
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.DataMigrationConsumer.Constants;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.DataMigration;
using Pi.SetMarketData.Domain.Models.Request.Sirius;
using Pi.SetMarketData.Domain.Models.Response.Sirius;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketData.DataMigrationConsumer.Handlers;

public class KafkaMessageHandler : IKafkaMessageHandler<string, string>
{
    private readonly string _dataTopicName;
    private readonly HttpRequestHelper<KafkaMessageHandler> _httpRequestHelper;
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IKafkaPublisher<string, string> _publisher;
    private readonly string _url;
    private SiriusMarketIndicatorRequest _payload = new();

    /// <summary>
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="configuration"></param>
    /// <param name="publisher"></param>
    /// <param name="logger"></param>
    public KafkaMessageHandler
    (
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IKafkaPublisher<string, string> publisher,
        ILogger<KafkaMessageHandler> logger
    )
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientKeys.Sirius);
        _publisher = publisher;
        _logger = logger;
        _httpRequestHelper = new HttpRequestHelper<KafkaMessageHandler>(httpClient, _logger);
        _dataTopicName = configuration.GetValue<string>(ConfigurationKeys.KafkaMigrationDataTopicName) ?? string.Empty;
        _url = "cgs/v2/market/indicator";
    }

    public async Task HandleAsync(ConsumeResult<string, string> consumeResult)
    {
        var migrationJob = MigrationJob.FromJson(consumeResult.Message.Value);

        _logger.LogDebug("Consumed message: {Message}", consumeResult.Message.Value);

        if (migrationJob == null
            || string.IsNullOrEmpty(migrationJob.Symbol)
            || string.IsNullOrEmpty(migrationJob.Venue))
        {
            _logger.LogError("Migration Job is null");
            return;
        }

        var from = DataManipulation.ToUnixTimestamp(migrationJob.DateTimeFrom);
        var to = DataManipulation.ToUnixTimestamp(migrationJob.DateTimeTo);

        _logger.LogDebug(
            "Migration Job - Symbol: {Symbol}, From: {From}, To: {To}, Venue: {Venue}",
            migrationJob.Symbol,
            from,
            to,
            migrationJob.Venue
        );

        // Have to handle venue
        SetPayload(migrationJob.Symbol, from, to, migrationJob.Venue);

        // Connect to Sirius
        var responseData = await HandleSiriusConnection(
            _httpRequestHelper,
            _url,
            _payload
        );

        MigrationData<SiriusMarketIndicatorResponse> migrationData =
            new() { MigrationJob = migrationJob, Response = responseData };

        var messageValue = DataManipulation.ToJsonString(migrationData);

        await _publisher.PublishAsync(
            _dataTopicName,
            new Message<string, string>
            {
                Key = migrationJob.Symbol ?? "",
                Value = messageValue
            }
        );
    }

    private void SetPayload(string? symbol, long from, long to, string? venue)
    {
        _payload = new SiriusMarketIndicatorRequest
        {
            // Fix data
            CandleType = "1min",
            CompleteTradingDay = false,
            Limit = 500,
            FromTimestamp = from,
            // Fix data
            Indicators = new RequestIndicators(),
            Symbol = symbol,
            ToTimestamp = to,
            Venue = venue
        };
    }

    private static async Task<SiriusMarketIndicatorResponse?> HandleSiriusConnection(
        IHttpRequestHelper client,
        string url,
        SiriusMarketIndicatorRequest payload
    )
    {
        var jsonPayload = DataManipulation.ToJsonString(payload);
        StringContent content = new(jsonPayload, Encoding.UTF8);

        var response = await client.RequestByContent(
            url,
            content,
            HttpMethod.Post
        );

        if (response == null) return null;

        var result = await AsyncResponseReader.StreamToString(response);
        return string.IsNullOrEmpty(result) ? null : SiriusMarketIndicatorResponse.FromJson(result);
    }
}