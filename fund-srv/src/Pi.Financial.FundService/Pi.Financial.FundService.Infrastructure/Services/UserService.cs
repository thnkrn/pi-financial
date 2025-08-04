using System.Net;
using Microsoft.Extensions.Logging;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Client;
using Pi.Client.UserService.Model;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Services.UserService;
using UserV2ApiException = Pi.Client.UserSrvV2.Client.ApiException;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserApi _userApi;
    private readonly IUserMigrationApi _userMigrationApi;
    private readonly ILogger<UserService> _logger;
    private readonly IDocumentApi _documentApi;
    private readonly Pi.Client.UserSrvV2.Api.IUserApi _userApi2;
    private readonly IFeatureService _featureService;
    private readonly Pi.Client.UserSrvV2.Api.IBankAccountApi _userV2BankAccountApi;

    public UserService
    (
        IUserApi userApi,
        IUserMigrationApi userMigrationApi,
        ILogger<UserService> logger,
        IDocumentApi documentApi,
        Pi.Client.UserSrvV2.Api.IUserApi userApi2,
        IFeatureService featureService,
        Pi.Client.UserSrvV2.Api.IBankAccountApi userV2BankAccountApi
    )
    {
        _userApi = userApi;
        _userMigrationApi = userMigrationApi;
        _logger = logger;
        _documentApi = documentApi;
        _userApi2 = userApi2;
        _featureService = featureService;
        _userV2BankAccountApi = userV2BankAccountApi;
    }

    public async Task<string> GetCustomerIdByUserId(string userId)
    {
        var user = await _userApi.GetUserByIdOrCustomerCodeV2Async(userId) ?? throw new InvalidDataException($"user not found with id: {userId}");

        return user.Data.CustomerId;
    }

    public async Task<IEnumerable<string>> GetCustomerCodesByUserId(Guid userId)
    {
        try
        {
            var response = await _userApi.InternalUserIdGetAsync(userId.ToString());
            return response.Data.CustCodes.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call User service InternalUserIdGetAsync with Exception ${Exception}", e.Message);
            return Array.Empty<string>();
        }
    }

    public async Task<IEnumerable<string>> GetFundCustomerCodesByUserId(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _userMigrationApi.InternalGetTradingAccountV2Async(userId, cancellationToken);

            return response.Data.Where(q => q.TradingAccounts.Any(x =>
                    x.Product == PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Funds))
                .Select(q => q.CustomerCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call GetFundCustomerCodesByUserId with Exception ${Exception}", e.Message);
            return [];
        }
    }

    public async Task<Guid?> GetUserIdByCustomerCode(string customerCode, CancellationToken cancellationToken = default)
    {
        try
        {
            Guid? userId;
            if (_featureService.IsOn(Features.SsoPhase3))
            {
                var response = await _userApi2.InternalV1UsersGetAsync(accountId: customerCode, cancellationToken: cancellationToken);
                var parsedGuid = Guid.Parse(response.Data[0].Id);
                userId = parsedGuid;
            }
            else
            {
                var response = await _userMigrationApi.InternalUserIdByCustomerCodeGetAsync(customerCode, cancellationToken);
                userId = response.Data;
            }

            return userId;
        }
        catch (ApiException e) when (e.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<string?> GetCitizenIdByUserId(Guid userId)
    {
        try
        {
            string? citizenId;
            if (_featureService.IsOn(Features.SsoPhase3))
            {
                var response = await _userApi2.InternalV1UsersGetAsync(ids: userId.ToString());
                citizenId = response.Data[0].CitizenId;
            }
            else
            {
                var response = await _userApi.InternalUserIdCitizenIdGetAsync(userId.ToString()) ?? throw new InvalidDataException($"user not found with userId: {userId}");
                citizenId = response.Data.CitizenId;

            }

            return citizenId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call User service InternalUserIdCitizenIdGetAsync with Exception ${Exception}", e.Message);
            return null;
        }
    }


    ///<summary>
    /// Returns list of <c>userId</c>'s documents. Will return empty list and log the error if failed to get documents from user service for some reason.
    ///</summary>
    public async Task<IEnumerable<PiUserApplicationModelsDocumentDocumentDto>> GetDocumentByUserId(Guid userId)
    {
        try
        {
            var response = await _documentApi.GetDocumentsByUserIdAsync(userId.ToString());
            return response.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "Unable to call User service GetDocumentsByUserIdAsync with Exception ${Exception}", e.Message);
            return Enumerable.Empty<PiUserApplicationModelsDocumentDocumentDto>();
        }

    }
    public async Task<AtsBankAccounts> GetAtsBankAccountsByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        var redemptionBankAccount = await GetAtsBankAccountForWithdrawalByCustomerCodeAsync(customerCode, cancellationToken);
        var subscriptionBankAccount = await GetAtsBankAccountForDepositByCustomerCodeAsync(customerCode, cancellationToken);
        return new AtsBankAccounts(redemptionBankAccount, subscriptionBankAccount);
    }

    private async Task<List<BankAccount>> GetAtsBankAccountForDepositByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await GetAtsBankAccountByCustomerCodeAsync(customerCode, "deposit", cancellationToken);
    }

    private async Task<List<BankAccount>> GetAtsBankAccountForWithdrawalByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await GetAtsBankAccountByCustomerCodeAsync(customerCode, "withdrawal", cancellationToken);
    }

    private async Task<List<BankAccount>> GetAtsBankAccountByCustomerCodeAsync(string customerCode, string purpose, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _userV2BankAccountApi.InternalV2BankAccountDepositWithdrawGetAsync(customerCode, purpose, ProductName.Funds.ToString(), cancellationToken);
            return response.Data.Select((x, i) =>
            {
                string defaultBankBranchCode = x.BankCode == "002" ? "0001" : "00000";
                string bankBranchCode = string.IsNullOrEmpty(x.BankBranchCode) || x.BankBranchCode == "0000"
                    ? defaultBankBranchCode
                    : x.BankBranchCode;
                return new BankAccount(x.BankCode, x.BankAccountNo, bankBranchCode, i == 0, x.PaymentToken);
            }).ToList();
        }
        catch (UserV2ApiException e) when (e.ErrorCode == (int)HttpStatusCode.BadRequest)
        {
            _logger.LogError(e, "Unable to call User service V2: InternalV1BankAccountDepositWithdrawGetAsync with Exception ${Exception}", e.Message);
            return [];
        }
    }
}
