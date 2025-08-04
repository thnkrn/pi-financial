using Pi.User.Application.Services.Onboard;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Model;
using Microsoft.Extensions.Logging;

namespace Pi.User.Infrastructure.Services;

public class OnboardTradingAccountService : IOnboardTradingAccountService
{
    private readonly ITradingAccountApi _tradingAccountApi;

    public OnboardTradingAccountService(ITradingAccountApi tradingAccountApi)
    {
        _tradingAccountApi = tradingAccountApi;
    }

    public async Task<List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount>>
        GetTradingAccountListByCustomerCodeAsync(string customerCode, bool withBankAccounts = false,
            bool withExternalAccounts = false, CancellationToken cancellationToken = default(CancellationToken))
    {
        var result = await _tradingAccountApi.InternalGetTradingAccountListByCustomerCodeV2Async(
            customerCode,
            withBankAccounts,
            withExternalAccounts,
            cancellationToken);
        return result.Data;
    }

    public async Task<List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode>>
        GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(string identificationNo,
            CancellationToken cancellationToken = default(CancellationToken))
    {
        var result =
            await _tradingAccountApi.InternalGetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(
                identificationNo, cancellationToken);
        return result.Data;
    }
}