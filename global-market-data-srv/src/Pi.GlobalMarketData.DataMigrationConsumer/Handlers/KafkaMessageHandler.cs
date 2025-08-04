using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.DataMigrationConsumer.Constants;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Models.DataMigration;
using Pi.GlobalMarketData.Domain.Models.Request.Velexa;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Handlers;
public class KafkaMessageHandler : IKafkaMessageHandler<string, string>
{
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IKafkaPublisher<string, string> _kafkaPublisher;
    private readonly HttpClient _httpClient;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaTokenGenerator;
    private string _token = "";
    private readonly int _tokenExpireInSecond = 3600;
    private readonly string? _kafkaTopic;


    public KafkaMessageHandler
    (
        ILogger<KafkaMessageHandler> logger,
        IKafkaPublisher<string, string> kafkaPublisher,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator
    )
    {
        _kafkaPublisher = kafkaPublisher;
        _kafkaTopic = configuration[ConfigurationKeys.KafkaMigrationDataTopicName]
                    ?? throw new InvalidOperationException("TopicName is not configured.");
        _httpClient = httpClientFactory.CreateClient(HttpClientKeys.VelexaHttpApi);
        _logger = logger;
        _velexaTokenGenerator = velexaHttpApiJwtTokenGenerator;
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

        var from = DataManipulation.ToUnixTimestamp(migrationJob.DateTimeFrom) * 1000;
        var to = DataManipulation.ToUnixTimestamp(migrationJob.DateTimeTo) * 1000;

        _logger.LogDebug(
            "Migration Job - Symbol: {Symbol}, From: {From}, To: {To}, Venue: {Venue}",
            migrationJob.Symbol,
            from,
            to,
            migrationJob.Venue
        );

        // Have to handle venue
        var payload = new OhlcVelexaRequest
        {
            From = from,
            To = to,
            Size = "5000",
            Type = OhlcRequestType.Trades.Value
        };
        var url = "/md/3.0/ohlc/{symbolId}/{duration}";
        var symbolId = migrationJob.Symbol + "." + migrationJob.Venue;

        // Connect to Velexa
        var responseData = await HandleVelexaConnection(
            url,
            symbolId,
            OhlcRequestDuration.D60.Value,
            payload
        );

        if (responseData == null)
        {
            _logger.LogError("Failed to get data from Velexa");
            return;
        }

        MigrationData<List<object>?> migrationData =
            new() { MigrationJob = migrationJob, Response = responseData };

        var messageValue = DataManipulation.ToJsonString(migrationData);
        await PublishToKafka(migrationJob.Symbol, messageValue);

        _logger.LogDebug(
            "Delivered '{Delivered}' with key '{Key}'",
            messageValue,
            migrationJob.Symbol
        );

    }

    protected async Task<List<object>?> HandleVelexaConnection(
        string url,
        string? symbolId,
        string duration,
        OhlcVelexaRequest payload
    )
    {
        try
        {
            if (_token == "" || _velexaTokenGenerator.IsTokenExpired(_token))
            {
                _logger.LogDebug("Token not found or expired, create new one");
                _token = _velexaTokenGenerator.GenerateJwtToken(_tokenExpireInSecond);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token");
            return [];
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var _url = url.Replace("{symbolId}", symbolId).Replace("{duration}", duration);

        // Build query parameters
        var queryParams = new List<string>
        {
            $"from={payload.From}",
            $"to={payload.To}",
            $"size={payload.Size}",
            $"type={payload.Type}",
        };
        _url = $"{_url}?{string.Join("&", queryParams)}";
        var response = await _httpClient.GetAsync(_url);

        if (response == null) return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result)) return null;

        return JsonConvert.DeserializeObject<List<object>?>(result);
    }

    private async Task PublishToKafka(string symbol, string jsonResponse)
    {
        // Publish to Kafka
        if (!string.IsNullOrEmpty(_kafkaTopic))
            await _kafkaPublisher.PublishAsync(
                _kafkaTopic,
                new Message<string, string> { Key = symbol ?? "", Value = jsonResponse }
            );
    }
}
