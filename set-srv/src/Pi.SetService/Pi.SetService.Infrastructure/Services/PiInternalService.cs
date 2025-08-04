using Microsoft.Extensions.Options;
using Pi.Client.PiInternal.Api;
using Pi.Client.PiInternal.Client;
using Pi.Common.Features;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.PiInternalService;
using Pi.SetService.Infrastructure.Factories;
using Pi.SetService.Infrastructure.Options;

namespace Pi.SetService.Infrastructure.Services;

public class PiInternalService(
    IBackOfficeApi backOfficeApi,
    IFeatureService featureService,
    IOptions<PiInternalServiceOptions> options
) : IPiInternalService
{
    private const string DateFormat = "yyyyMMdd";

    public async Task<List<Trade>> GetTradeHistories(string accountNumber, DateOnly startDate, DateOnly endDate,
        CancellationToken ct = default
    )
    {
        try
        {
            var apiKey = Guid.Parse(options.Value.ApiKey);
            var apiResponse =
                await backOfficeApi.ApiBackOfficeOrderTradesGetAsync(startDate.ToString(DateFormat),
                    endDate.ToString(DateFormat),
                    accountNumber, apiKey, ct);
            var result = apiResponse.Data.Select(EntityFactory.NewTrade).ToList();

            return result;
        }
        catch (ApiException e)
        {
            return HandleApiException<List<Trade>>(e, []);
        }
    }

    public async Task<BackofficeAvailableBalance?> GetBackofficeAvailableBalance(string accountNumber,
        CancellationToken ct = default)
    {
        try
        {
            if (featureService.IsOn(Features.OnePortMaintenance))
            {
                return null;
            }

            var apiKey = Guid.Parse(options.Value.ApiKey);
            var apiResponse = await backOfficeApi.ApiBackOfficeAUMByAccountGetAsync(apiKey, accountNumber, ct);
            var aum = apiResponse.Data.FirstOrDefault();
            if (aum == null)
            {
                return null;
            }

            return new BackofficeAvailableBalance
            {
                TradingAccountNo = aum.Account,
                AccountNo = aum.Account.Replace("-", ""),
                CashBalance = (decimal)aum.CashBalance,
                ArTrade = (decimal)aum.ArTrade,
                ApTrade = (decimal)aum.ApTrade,
                MarketValue = (decimal)aum.MarketValue,
                PostDateTime = aum.PostDate
            };
        }
        catch (ApiException e)
        {
            return HandleApiException<BackofficeAvailableBalance?>(e, null);
        }
    }

    private static TResult HandleApiException<TResult>(ApiException exception, TResult result)
    {
        return exception.ErrorCode switch
        {
            401 => throw new UnauthorizedAccessException(exception.Message, exception),
            404 => result,
            _ => throw exception
        };
    }
}
