using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.Application.Interfaces.MarketInstrumentInfo;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketInstrumentInfo;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Pi.GlobalMarketData.Domain.Models.Response.Velexa;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketInstrumentInfo;

public class MarketInstrumentInfoRequestHandler : PostMarketInstrumentInfoRequestAbstractHandler
{
    private readonly ILogger<MarketInstrumentInfoRequestHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly IVelexaHttpApiJwtTokenGenerator _velexaTokenGenerator;
    private readonly int _tokenExpireInSecond = 300; // 5 Min
    private readonly ICacheService _cacheService;
    private readonly IVenueMappingHelper _venueMappingHelper;
    private readonly IEntityCacheService _entityCacheService;

    public MarketInstrumentInfoRequestHandler(
        IHttpClientFactory httpClientFactory,
        ILogger<MarketInstrumentInfoRequestHandler> logger,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IVenueMappingHelper venueMappingHelper,
        ICacheService cacheService,
        IEntityCacheService entityCacheService
    )
    {
        _httpClient = httpClientFactory.CreateClient("Velexa HTTP API");
        _logger = logger;
        _velexaTokenGenerator = velexaHttpApiJwtTokenGenerator;
        _venueMappingHelper = venueMappingHelper;
        _cacheService = cacheService;
        _entityCacheService = entityCacheService;
    }

    protected override async Task<PostMarketInstrumentInfoResponse> Handle(PostMarketInstrumentInfoRequest request, CancellationToken cancellationToken)
    {
        var result = new MarketInstrumentInfoResponse();

        try
        {
            string symbol = request.Data.Symbol?.ToLower() ?? string.Empty;
            string exchange = await _venueMappingHelper.GetExchangeNameFromVenueMapping(request.Data.Venue);
            exchange = exchange.ToLower();

            var whitelist = await _entityCacheService.GetWhiteList(symbol, exchange);

            if (whitelist == null)
                throw new InvalidOperationException("No data found from whitelist, failed to get result from service");

            try
            {
                string symbolId = $"{whitelist.Symbol}.{whitelist.Exchange}";
                string _token = string.Empty;
                VelexaInstrument? instrumentData = null;
                VelexaInstrumentSpecification? instrumentSpecData = null;

                instrumentData = await _cacheService.GetAsync<VelexaInstrument>($"VelexaInstrument:InstrumentInfo:{symbolId}");
                if (instrumentData == null)
                {
                    //TODO: change endpoint to use /md/{version}/symbols/{symbolId} to query by symbol
                    string requestInstrumentUrl = $"/md/3.0/exchanges/{whitelist.Exchange}";
                    _token = _velexaTokenGenerator.GenerateJwtToken(_tokenExpireInSecond);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    var velexaInstrumentResponse = await _httpClient.GetFromJsonAsync<List<VelexaInstrument>>(requestInstrumentUrl);
                    if (velexaInstrumentResponse == null)
                    {
                        _logger.LogError("No data received for exchange {Exchange}", whitelist.Exchange);
                        return new PostMarketInstrumentInfoResponse(result);
                    }
                    else
                        instrumentData = velexaInstrumentResponse.First(e => e.SymbolId == symbolId) ?? null;

                    if (instrumentData != null)
                        await _cacheService.SetAsync<VelexaInstrument>($"VelexaInstrument:InstrumentInfo:{symbolId}", instrumentData, TimeSpan.FromHours(8));
                }

                instrumentSpecData = await _cacheService.GetAsync<VelexaInstrumentSpecification>($"VelexaInstrument:Specification:{symbolId}");
                if (instrumentSpecData == null)
                {
                    if(string.IsNullOrEmpty(_token))
                    {
                        _token = _velexaTokenGenerator.GenerateJwtToken(_tokenExpireInSecond);
                    }
                    string requestSpecificicationUrl = $"/md/3.0/symbols/{symbolId}/specification";
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    instrumentSpecData = await _httpClient.GetFromJsonAsync<VelexaInstrumentSpecification>(requestSpecificicationUrl);

                    if (instrumentSpecData != null)
                        await _cacheService.SetAsync<VelexaInstrumentSpecification>($"VelexaInstrument:Specification:{symbolId}", instrumentSpecData, TimeSpan.FromHours(8));
                }

                if (instrumentData != null && instrumentSpecData != null)
                    result = MarketInstrumentInfoService.GetResult(instrumentData, instrumentSpecData);

                return new PostMarketInstrumentInfoResponse(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Handle Http request failed.");
                throw new InvalidOperationException("Failed to get result from service", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get result from service", ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}