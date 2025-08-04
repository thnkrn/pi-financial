using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Services;

public class OnboardService : IOnboardService
{
    private readonly IBankAccountApi _bankAccountApi;
    private readonly ICustomerInfoApi _customerInfoApi;
    private readonly ITradingAccountApi _tradingAccountApi;
    private readonly ILogger<OnboardService> _logger;

    public OnboardService(
        IBankAccountApi bankAccountApi,
        ICustomerInfoApi customerInfoApi,
        ILogger<OnboardService> logger, ITradingAccountApi tradingAccountApi)
    {
        _bankAccountApi = bankAccountApi;
        _customerInfoApi = customerInfoApi;
        _logger = logger;
        _tradingAccountApi = tradingAccountApi;
    }

    public async Task<BankAccount?> GetBankAccount(string customerCode, Product product,
        TransactionType transactionType)
    {
        try
        {
            var resp = await _bankAccountApi.GetBankAccountForDepositWithdrawalByProductNameAsync(
                customerCode,
                product.ToString(),
                transactionType == TransactionType.Deposit ? "deposit" : "withdrawal");

            return new BankAccount(resp.Data.BankCode, resp.Data.BankAccountNo, resp.Data.BankBranchCode);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "OnboardService:GetTradingAccountListByCustomerCode: Unable to get bank account with CustomerCode {CustomerCode} ",
                customerCode);
            throw;
        }
    }

    public async Task<CustomerInfo> GetCustomerInfo(string customerCode)
    {
        try
        {
            var response = await _customerInfoApi.InternalGetCustomerInfoByCustomerCodeAsync(customerCode);
            if (response == null)
            {
                throw new InvalidDataException("Customer Info Not Found");
            }

            return new CustomerInfo(
                response.Data.CustomerCode,
                response.Data.BasicInfo.Name.FirstnameTh,
                response.Data.BasicInfo.Name.LastnameTh,
                response.Data.BasicInfo.Name.FirstnameEn,
                response.Data.BasicInfo.Name.LastnameEn,
                response.Data.IdentificationCard.Number
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "OnboardService:GetCustomerInfoByCustomerCode: Unable to get customer info with CustCode {CustomerCode} ",
                customerCode);
            throw;
        }
    }

    public async Task<string?> GetMarketingId(string customerCode, string tradingAccount)
    {
        try
        {
            var response = await _tradingAccountApi.InternalGetMarketingInfosAsync(customerCode);

            return response.Data
                .Where(t => t.TradingAccountNo.Replace("-", string.Empty) == tradingAccount)
                .Select(t => t.MarketingId)
                .FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "OnboardService:InternalGetMarketingInfosAsync: Unable to get marketingId info with CustCode {CustomerCode}",
                customerCode);
            throw;
        }
    }
}