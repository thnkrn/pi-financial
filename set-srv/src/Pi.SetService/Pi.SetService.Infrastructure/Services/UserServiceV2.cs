using Microsoft.Extensions.Logging;
using Pi.Client.UserSrvV2.Api;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Factories;

namespace Pi.SetService.Infrastructure.Services;

public class UserServiceV2(IUserApi userApi, ITradingAccountApi tradingAccountApi, ILogger<UserServiceV2> logger) : IUserService, IOnboardService
{
    private readonly string[] _lendingAccountTypeCodes = ["LC", "BB", "LH"];
    private readonly string[] _borrowAccountTypeCodes = ["BH", "BC"];

    public async Task<IEnumerable<string>> GetCustomerCodesByUserId(Guid userId)
    {
        try
        {
            var resp = await userApi.InternalV1UsersGetAsync(ids: userId.ToString());
            if (resp.Data.Count == 0)
            {
                return [];
            }

            var user = resp.Data.FirstOrDefault(q => q.Id == userId.ToString());

            return user?.CustCodes ?? [];
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't GetCustomerCodesByUserId with error: {ErrMsg}", e.Message);
            return [];
        }
    }

    public async Task<Guid?> GetUserIdByCustCode(string custCode, CancellationToken contextCancellationToken)
    {
        try
        {
            var resp = await userApi.InternalV1UsersGetAsync(accountId: custCode, cancellationToken: contextCancellationToken);
            if (resp.Data.Count == 0)
            {
                return null;
            }

            return Guid.Parse(resp.Data.First().Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't GetUserIdByCustCode with error: {ErrMsg}", e.Message);
            return null;
        }
    }

    public async Task<TradingAccount?> GetSetTradingAccountByCustCodeAsync(string custCode, TradingAccountType tradingAccountType,
        CancellationToken cancellationToken = default)
    {
        var tradingAccounts = await GetSetTradingAccountsByCustCodeAsync(custCode, cancellationToken);

        return tradingAccounts.Find(account => account.TradingAccountType == tradingAccountType);
    }

    public Task<List<TradingAccount>> GetSetTradingAccountsByCustCodeAsync(string custCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TradingAccount>> GetSetTradingAccountsByUserIdAsync(Guid userId, string custCode, CancellationToken cancellationToken = default)
    {
        var response = await tradingAccountApi.InternalV1TradingAccountsGetAsync(userId.ToString(), "N", cancellationToken: cancellationToken);
        var tradeAccountResponse = response.Data.Find(q => q.CustomerCode == custCode);
        if (tradeAccountResponse == null)
        {
            return [];
        }

        var result = new List<TradingAccount>();
        tradeAccountResponse.TradingAccounts.ForEach(setAccount =>
        {
            var accountType = EntityFactory.NewTradingAccountType(setAccount.AccountTypeCode, false);
            if (accountType == null) return;

            if (new[]
                {
                    TradingAccountType.CreditBalance,
                    TradingAccountType.Cash,
                    TradingAccountType.CashBalance
                }
                .Contains((TradingAccountType)accountType))
            {
                result.Add(new TradingAccount(Guid.Parse(setAccount.Id), tradeAccountResponse.CustomerCode, setAccount.TradingAccountNo, (TradingAccountType)accountType)
                {
                    SblRegistered = tradeAccountResponse.TradingAccounts.Exists(account =>
                        account.TradingAccountNo == setAccount.TradingAccountNo &&
                        (_lendingAccountTypeCodes.Contains(account.AccountTypeCode) ||
                         _borrowAccountTypeCodes.Contains(account.AccountTypeCode)))
                });
            }
        });

        return result;
    }
}
