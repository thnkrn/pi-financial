using System.Net;
using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Client;
using Pi.Client.OnboardService.Model;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.OnboardService;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class OnboardService : IOnboardService
{
    private readonly IOpenAccountApi _openAccountApi;
    private readonly ICrsApi _crsApi;
    private readonly ITradingAccountApi _tradingAccountApi;
    private readonly IBankAccountApi _bankAccountApi;
    private readonly IDopaApi _dopaApi;
    private readonly Pi.Client.OnboardService.Api.IUserDocumentApi _onboardUserDocumentApi;
    private readonly ILogger<OnboardService> _logger;

    public OnboardService(
        IOpenAccountApi openAccountApi,
        ICrsApi crsApi,
        ITradingAccountApi tradingAccountApi,
        IBankAccountApi bankAccountApi,
        IDopaApi dopaApi,
        IUserDocumentApi userDocumentApi,
        ILogger<OnboardService> logger)
    {
        _openAccountApi = openAccountApi;
        _crsApi = crsApi;
        _tradingAccountApi = tradingAccountApi;
        _bankAccountApi = bankAccountApi;
        _dopaApi = dopaApi;
        _onboardUserDocumentApi = userDocumentApi;
        _logger = logger;
    }

    public async Task UpdateOpenFundAccountStatus(Guid openAccountRequestId, OpenFundAccountStatus status)
    {
        await _openAccountApi.CallbackFundAccountAsync(
            new PiOnboardServiceApplicationCommandsCallbackFundAccountRequest(
                openAccountRequestId,
                status is OpenFundAccountStatus.SUCCESS ? PiOnboardServiceApplicationCommandsCallbackFundAccountRequest.StatusEnum.Completed : PiOnboardServiceApplicationCommandsCallbackFundAccountRequest.StatusEnum.Failed
            )
        );
    }

    public async Task<Crs> GetUserCrsByUserId(Guid userId)
    {
        var resp = await _crsApi.UserCrsAsync(userId.ToString());
        return new Crs(
                resp.Data.PlaceOfBirthCountry,
                resp.Data.PlaceOfBirthCity,
                resp.Data.TaxResidenceInCountriesOtherThanTheUS,
                resp.Data.Details.Select(
                    x => new CrsDetail(
                        x.ResidenceCountry,
                        x.Tin,
                        x.NoTinReason is not null
                            ? Enum.Parse<CrsReason>(x.NoTinReason.ToString()!)
                            : null,
                        x.NoTinReasonDesc
                    )
                ).ToList(),
                resp.Data.DeclarationDate);
    }

    public async Task<TradingAccountInfo?> GetMutualFundTradingAccountByCustCodeAsync(string custCode, CancellationToken cancellationToken = default)
    {
        var response = await _tradingAccountApi.InternalGetTradingAccountListByCustomerCodeAsync(custCode, cancellationToken: cancellationToken);
        var tradingAccounts = response.Data;
        var mutualFundAccount = tradingAccounts.Find(q => q.AccountTypeCode.ToUpper() == "UT");

        if (mutualFundAccount == null) return null;

        return new TradingAccountInfo
        {
            Id = mutualFundAccount.Id,
            AccountNo = mutualFundAccount.TradingAccountNo.Replace("-", ""),
            SaleLicense = mutualFundAccount.SaleLicense,
            OpenDate = mutualFundAccount.OpenDate.HasValue ? DateOnly.FromDateTime(mutualFundAccount.OpenDate.Value) : null
        };
    }

    public async Task<BankAccount?> GetATSBankAccountForDepositByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await GetATSBankAccountByCustomerCodeAsync(customerCode, "deposit", cancellationToken);
    }

    public async Task<BankAccount?> GetATSBankAccountForWithdrawalByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await GetATSBankAccountByCustomerCodeAsync(customerCode, "withdrawal", cancellationToken);
    }

    private async Task<BankAccount?> GetATSBankAccountByCustomerCodeAsync(string customerCode, string purpose, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _bankAccountApi.GetBankAccountForDepositWithdrawalByProductNameAsync(customerCode, ProductName.Funds.ToString(), purpose, cancellationToken: cancellationToken);
            string defaultBankBranchCode = response.Data.BankCode == "002" ? "0001" : "00000";
            string bankBranchCode = string.IsNullOrEmpty(response.Data.BankBranchCode) || response.Data.BankBranchCode == "0000" ? defaultBankBranchCode : response.Data.BankBranchCode;
            return new BankAccount(response.Data.BankCode, response.Data.BankAccountNo, bankBranchCode, true, response.Data.PaymentToken);
        }
        catch (ApiException e) when (e.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<AtsBankAccounts> GetATSBankAccountsByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        var redemptionBankAccount = await GetATSBankAccountForWithdrawalByCustomerCodeAsync(customerCode, cancellationToken);
        var subscriptionBankAccount = await GetATSBankAccountForDepositByCustomerCodeAsync(customerCode, cancellationToken);
        var redemptionBankAccounts = new List<BankAccount>();
        if (redemptionBankAccount != null)
        {
            redemptionBankAccounts.Add(redemptionBankAccount);
        }
        var subscriptionBankAccounts = new List<BankAccount>();
        if (subscriptionBankAccount != null)
        {
            subscriptionBankAccounts.Add(subscriptionBankAccount);
        }

        return new AtsBankAccounts(redemptionBankAccounts, subscriptionBankAccounts);
    }

    public async Task<DateTime?> GetDopaSuccessInfoByUserId(Guid userId)
    {
        try
        {
            var response = await _dopaApi.InternalGetDopaSuccessInfoAsync(userId);
            return response.Data.DateTime;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to GetDopaSuccessInfoByUserId, {Exception}", e.Message);
            return null;
        }
    }

    public async Task<List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto>> GetDocumentByUserId(
        Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _onboardUserDocumentApi.GetDocumentsByUserIdAsync(userId, cancellationToken);
            return response.Data;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call User service GetDocumentsByUserIdAsync with Exception ${Exception}", e.Message);
            return [];
        }
    }
}
