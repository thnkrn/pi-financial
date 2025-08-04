using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Infrastructure.Services;

public class MarketDataService(IMarketDataApi marketDataApi,
    ISiriusApi siriusApi,
    IFeatureService featureService,
    ILogger<MarketDataService> logger) : IMarketDataService
{
    public async Task<List<Ticker>> GetTicker(string? sid, List<string> symbols)
    {
        const int batchSize = 100;
        var tickers = new List<Ticker>();

        for (var i = 0; i < symbols.Count; i += batchSize)
        {
            var batch = symbols.Skip(i).Take(batchSize);
            var request = new MarketTickerRequest
            {
                Param = batch.Select(symbol => new MarketTickerParameter(symbol, Venue.Derivative.ToString())).ToList()
            };

            try
            {
                MarketTickerResponse response;
                if (featureService.IsOn(Features.AppSynthMarketData) || string.IsNullOrEmpty(sid))
                {
                    response = await marketDataApi.MarketTickerAsync(request);
                }
                else
                {
                    response = await siriusApi.CgsV2MarketTickerPostAsync(sid, request);
                }
                if (response != null && response.Response?.Data.Count > 0)
                {
                    tickers.AddRange(response.Response.Data);
                }
            }
            catch (Exception e)
            {
                throw HandleException(e, "GetMarketData");
            }
        }

        return tickers;
    }

    private SiriusApiException HandleException(Exception e, string methodName)
    {
        logger.LogError("MarketDataService {MethodName}: {Message}", methodName, e.Message);
        return new SiriusApiException($"MarketDataService {methodName}: {e.Message}");
    }
}