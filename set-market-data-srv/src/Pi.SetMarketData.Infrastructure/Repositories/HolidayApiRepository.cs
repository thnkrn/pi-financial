using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Domain.AggregatesModels.HolidayAggregate;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.Holiday;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Polly;
using Polly.Retry;

namespace Pi.SetMarketData.Infrastructure.Repositories;

public class HolidayApiRepository : IHolidayApiRepository
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<HolidayApiRepository> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly string? _settradeHolidayApiInternalEndpoint;
    private readonly string? _settradeHolidayApiSecureEndpoint;
    private readonly bool _useSecureEndpoint;

    public HolidayApiRepository(IHttpClientFactory clientFactory, ILogger<HolidayApiRepository> logger,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _useSecureEndpoint = configuration.GetValue(ConfigurationKeys.SettradeHolidayApiUseSecureEndpoint, true);
        _settradeHolidayApiInternalEndpoint =
            configuration.GetValue(ConfigurationKeys.SettradeHolidayApiInternalEndpoint,
                "/internal/calendar/is-holiday");
        _settradeHolidayApiSecureEndpoint = configuration.GetValue(ConfigurationKeys.SettradeHolidayApiSecureEndpoint,
            "/secure/calendar/is-holiday");
        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, _) =>
                {
                    _logger.LogWarning("Attempting retry {RetryAttempt} after {Delay}ms delay due to {Reason}",
                        retryAttempt, timespan.TotalMilliseconds,
                        outcome.Exception?.Message ?? outcome.Result.ReasonPhrase);
                }
            );
    }

    public async Task<bool> IsHoliday(string date)
    {
        return await CheckHoliday(date, _useSecureEndpoint);
    }

    private async Task<bool> CheckHoliday(string date, bool useSecureEndpoint)
    {
        var client = _clientFactory.CreateClient("HolidayApi");
        var endpoint = useSecureEndpoint ? _settradeHolidayApiSecureEndpoint : _settradeHolidayApiInternalEndpoint;
        var url = $"/{endpoint?.Trim('/')}/{date}";

        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await client.GetAsync(url));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<HolidayApiResponse<bool>>();
            return result?.Data ?? false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "HTTP request failed when checking holiday for date {Date} using {Endpoint} endpoint",
                date, endpoint);
            throw new InfrastructureException($"Failed to check holiday due to HTTP error using {endpoint} endpoint",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred when checking holiday for date {Date} using {Endpoint} endpoint",
                date, endpoint);
            throw new InfrastructureException(
                $"Unexpected error occurred when checking holiday using {endpoint} endpoint", ex);
        }
    }
}