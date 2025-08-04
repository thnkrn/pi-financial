using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.PiItBackoffice.Api;
using Pi.Financial.Client.PiItBackoffice.Client;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Infrastructure.Models;
using Pi.Financial.FundService.Infrastructure.Options;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class ItBackofficeService(
    IBackOfficeApi backOfficeApi,
    IDistributedCache cache,
    IOptions<ItBackofficeOptions> options,
    ILogger<ItBackofficeService> logger) : IItBackofficeService
{
    private ItBackofficeOptions _options => options.Value;

    public async Task<List<BoFundAssetResponse>> GetAccountBalanceAsync(string accountNo,
        string amc,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string cacheKey = $"fund::{accountNo}-{amc}";
            byte[]? cachedValue = await cache.GetAsync(cacheKey, cancellationToken);
            if (cachedValue != null)
            {
                var cacheResult = JsonSerializer.Deserialize<List<BoFundAssetResponse>>(Encoding.UTF8.GetString(cachedValue));

                if (cacheResult != null)
                {
                    return cacheResult;
                }
            }

            var response = await backOfficeApi.ApiBackOfficeFundBalanceGetAsync(_options.ApiKey, accountNo, amc, cancellationToken: cancellationToken);
            var result = response.Select(q => new BoFundAssetResponse
            {
                AmcCode = q.AmcCode,
                AccountId = q.AccountID,
                UnitholderId = q.UnitholderID,
                FundCode = q.FundCode,
                Unit = q.UnitBalance != null ? (decimal)q.UnitBalance : 0,
                Amount = q.Amount != null ? (decimal)q.Amount : 0,
                RemainUnit = q.UnitBalance != null ? (decimal)q.UnitBalance : 0,
                RemainAmount = q.Amount != null ? (decimal)q.Amount : 0,
                PendingAmount = q.PendingAmount != null ? (decimal)q.PendingAmount : 0,
                PendingUnit = q.PendingUnit != null ? (decimal)q.PendingUnit : 0,
                AvgCost = q.AverageCost != null ? (decimal)q.AverageCost : 0,
                Nav = q.Nav != null ? (decimal)q.Nav : 0,
                NavDate = q.NavDate != null ? DateOnly.FromDateTime((DateTime)q.NavDate) : default
            }).ToList();

            if (result.Count != 0)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.CacheExpiration));

                await cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)), cacheEntryOptions, cancellationToken);
            }

            return result;
        }
        catch (ApiException e)
        {
            if (e.ErrorCode == 404)
            {
                return [];
            }

            logger.LogError(e, "Can't retrieve value from ItBackofficeApi");
            return [];
        }
    }
}
