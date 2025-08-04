using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Client.UserSrvV2.Model;
using Pi.GlobalEquities.Application.Services.User;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using UserAPIResponse =
    Pi.Client.UserService.Model.PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse;

namespace Pi.GlobalEquities.Infrastructure.Services.User;

public class UserService(
    IUserMigrationApi userApi,
    ITradingAccountApi tradingAccountV2Api)
    : IUserService
{
    public async Task<IEnumerable<IAccount>> GetGeAccounts(string userId, CancellationToken ct)
    {
        var guid = new Guid(userId);
        var v1Response = await userApi.InternalGetTradingAccountV2Async(guid, ct);

        return GetSupportedAccounts(v1Response, userId);
    }

    public async Task<IEnumerable<IAccount>> GetGeAccountsV2(string userId, CancellationToken ct)
    {
        var v2Response = await tradingAccountV2Api.InternalV1TradingAccountsGetAsync(userId, cancellationToken: ct);
        return GetSupportedAccounts(v2Response, userId);
    }

    private IEnumerable<IAccount> GetSupportedAccounts(
        UserAPIResponse tradingAccount,
        string userId)
    {
        foreach (var accountItem in tradingAccount.Data)
        {
            foreach (var acc in accountItem.TradingAccounts)
            {
                var account = acc.ExternalAccounts.FirstOrDefault(x => x.ProviderId == (int)Provider.Velexa);
                if (account == null)
                    continue;

                var accountId = account.Id.ToString();
                yield return new DomainModels.Account
                {
                    Id = accountId,
                    UserId = userId,
                    CustCode = accountItem.CustomerCode,
                    TradingAccountNo = acc.TradingAccountNo,
                    VelexaAccount = account.Account,
                    UpdatedAt = DateTime.UtcNow
                };
            }
        }
    }

    private IEnumerable<IAccount> GetSupportedAccounts(
        InternalV1TradingAccountsGet200Response tradingAccount,
        string userId)
    {
        var supportedAccounts = new List<IAccount>();

        foreach (var accountItem in tradingAccount.Data)
        {
            supportedAccounts.AddRange(
            from acc in accountItem.TradingAccounts
                //NOTE: need to compare AccountType to check if it GE Account since user service v2 returns ge account for every account type
            where string.Equals(acc.ProductName, "GlobalEquities", StringComparison.OrdinalIgnoreCase)
            let externalAccount = acc.ExternalAccounts
                .FirstOrDefault(x => x.ProviderId == (int)Provider.Velexa)
            where externalAccount != null
            select new Account
            {
                Id = externalAccount.Id,
                UserId = userId,
                CustCode = accountItem.CustomerCode,
                TradingAccountNo = acc.TradingAccountNo,
                VelexaAccount = externalAccount.Account,
                UpdatedAt = DateTime.UtcNow,
                EnableBuy = acc.EnableBuy == "1",
                EnableSell = acc.EnableSell == "1"
            });
        }

        return supportedAccounts;
    }

}
