using Microsoft.Extensions.Logging;
using Pi.Client.FundMarketData.Api;
using Pi.Client.FundMarketData.Client;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Infrastructure.Factories;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class MarketService : IMarketService
{
    private readonly IFundApi _fundMarketApi;
    private readonly ILogger<MarketService> _logger;
    private readonly IFeatureService _featureService;

    public MarketService(IFundApi fundMarketApi, ILogger<MarketService> logger, IFeatureService featureService)
    {
        _fundMarketApi = fundMarketApi;
        _logger = logger;
        _featureService = featureService;
    }

    public async Task<IEnumerable<FundInfo>> GetFundInfosAsync(IEnumerable<string> fundCodes, CancellationToken cancellationToken = default)
    {
        if (_featureService.IsOn(Features.MockFundMarket))
        {
            _logger.LogInformation("Market data mock return empty");
            return Array.Empty<FundInfo>();
        }

        var fundCodeList = fundCodes.ToList();
        var grouped = fundCodeList.Count <= 30 ? new List<List<string>>() { fundCodeList.ToList() } : SplitList(fundCodeList, 30);
        var result = await Task.WhenAll(grouped.Select(async data =>
        {
            try
            {
                var response = await _fundMarketApi.InternalFundsTradingProfilesPostAsync(data, cancellationToken);
                return response.Data.Select(EntityFactory.NewFundInfo).ToList();
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "GetFundInfosAsync \"{FundCode}\" failed with error {ErrMsg}",
                    string.Join(",", data), e.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetFundInfosAsync \"{FundCode}\" failed with error {ErrMsg}",
                    string.Join(",", data), e.Message);
                throw;
            }
        }));

        return result.Where(q => q != null).SelectMany(q => q!);
    }

    public async Task<FundInfo?> GetFundInfoByFundCodeAsync(string fundCode, CancellationToken cancellationToken = default)
    {
        if (_featureService.IsOn(Features.MockFundMarket))
        {
            _logger.LogInformation("Market data mock return null");
            return null;
        }

        try
        {
            var response = await _fundMarketApi.InternalFundsTradingProfilesPostAsync(new List<string> { fundCode }, cancellationToken);
            var result = response.Data.FirstOrDefault();

            return result != null ? EntityFactory.NewFundInfo(result) : null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetFundInfoByFundCodeAsync \"{FundCode}\" failed with error {ErrMsg}", fundCode, e.Message);
            return null;
        }
    }

    private static List<List<string>> SplitList(IReadOnlyCollection<string> array, int chunkSize)
    {
        var splitLists = new List<List<string>>();
        for (var i = 0; i < array.Count; i += chunkSize)
        {
            var chunk = array.Skip(i).Take(chunkSize).ToList();
            splitLists.Add(chunk);
        }
        return splitLists;
    }
}
