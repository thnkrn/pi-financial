using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;
using Microsoft.Diagnostics.Symbols;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.Models.Response.Velexa;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Polly;
using Polly.Retry;

namespace Pi.GlobalMarketData.Infrastructure.Helpers;
public class VelexaApiHelper : IVelexaApiHelper
{
    private readonly ILogger<VelexaApiHelper> _logger;
    private readonly ICacheService _cacheService;
    private readonly HttpClient _httpClient;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaTokenGenerator;
    private readonly int _tokenExpireInSecond = 300; // 5 Min
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly int _maxRetryAttemp = 2;
    public VelexaApiHelper(
        ICacheService cacheService,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IHttpClientFactory httpClientFactory,
        ILogger<VelexaApiHelper> logger
    )
    {
        _cacheService = cacheService;
        _velexaTokenGenerator = velexaHttpApiJwtTokenGenerator;
        _httpClient = httpClientFactory.CreateClient("Velexa HTTP API");
        _logger = logger;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(_maxRetryAttemp, retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} due to {exception.Message}");
                });
    }

    public async Task<string> getMinimumOrderSize(string symbol, string venue)
    {
        // Get cached velexaInstrumentSpecification
        var cahcedInstrument = await _cacheService.GetAsync<VelexaInstrumentSpecification>($"VelexaInstrument:Specification:{symbol}");

        // If not exists or velexaInstrument.MinimalQuantityIncrement, call velexaExchangeAPI
        if(cahcedInstrument == null) 
        {
            try
            {
                var velexaInstrumentSpec = await httpRequestSpecification(symbol, venue);
                cahcedInstrument = velexaInstrumentSpec;
            } catch(HttpRequestException ex) {
                _logger.LogError(ex, "Failed to make http request to velexa API");
                _ = retryInBackground(symbol, venue);
                return "0.00";
            }
            finally {
                await saveToCache(symbol, cahcedInstrument);
                
            }
        }
        return cahcedInstrument.LotSize;
    }

    private async Task<VelexaInstrumentSpecification> httpRequestSpecification(string symbol, string venue)
    {
        string requestInstrumentUrl = $"/md/3.0/symbols/{symbol}.{venue}/specification";
        var _token = _velexaTokenGenerator.GenerateJwtToken(_tokenExpireInSecond);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        var velexaInstrumentResponse = await _httpClient.GetFromJsonAsync<VelexaInstrumentSpecification>(requestInstrumentUrl);
        if (velexaInstrumentResponse == null)
        {
            _logger.LogError("No data received for symbol {Symbol}", symbol);
            return null;
        } 
        return velexaInstrumentResponse;
    }
    private async Task saveToCache(string symbol, VelexaInstrumentSpecification velexaInstrumentSpec)
    {
        await _cacheService.SetAsync<VelexaInstrumentSpecification>($"VelexaInstrument:Specification:{symbol}", velexaInstrumentSpec, TimeSpan.FromHours(8));
    }

    private async Task retryInBackground(string symbol, string venue)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await httpRequestSpecification(symbol, venue);
            
            if (response != null)
            {
                await saveToCache(symbol, response);
            }
        });
    }
}