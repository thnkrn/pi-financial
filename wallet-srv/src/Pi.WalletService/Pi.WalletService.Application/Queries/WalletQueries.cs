using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.Freewill.Model;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries;

[Serializable]
public class NoBankAccountFoundException : Exception
{
    public NoBankAccountFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public NoBankAccountFoundException()
    {
    }

    public NoBankAccountFoundException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected NoBankAccountFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class WalletQueries : IWalletQueries
{
    private readonly IGlobalTradeService _globalTradeService;
    private readonly IUserService _userService;
    private readonly IOnboardService _onboardService;
    private readonly ICustomerService _freewillCustomerService;
    private readonly IBankInfoService _bankInfoService;
    private readonly ISetTradeService _setTradeService;
    private readonly IValidationService _validationService;
    private readonly ILogger<WalletQueries> _logger;
    private readonly FeaturesOptions _featuresOptions;

    public WalletQueries(
        IGlobalTradeService globalTradeService,
        IUserService userService,
        IOnboardService onboardService,
        ICustomerService freewillService,
        IBankInfoService bankInfoService,
        ISetTradeService setTradeService,
        ILogger<WalletQueries> logger,
        IOptionsSnapshot<FeaturesOptions> featureOptions,
        IValidationService validationService)
    {
        _globalTradeService = globalTradeService;
        _userService = userService;
        _onboardService = onboardService;
        _freewillCustomerService = freewillService;
        _bankInfoService = bankInfoService;
        _setTradeService = setTradeService;
        _logger = logger;
        _validationService = validationService;
        _featuresOptions = featureOptions.Value;
    }

    public async Task<decimal> GetAvailableWithdrawalAmount(string userId, string custCode, Product product)
    {
        var (user, accountCode) = await GetAndValidateUserByProductType(userId, custCode, product);
        switch (product)
        {
            case Product.GlobalEquities:
                return await _globalTradeService.GetAvailableWithdrawalAmount(user.GlobalAccount,
                    Currency.USD.ToString());
            case Product.Cash:
            case Product.CashBalance:
            case Product.CreditBalance:
            case Product.CreditBalanceSbl:
            case Product.Funds:
                return await _freewillCustomerService.QueryWithdrawalBalance(accountCode!);
            case Product.Derivatives:
                var freewillWithdrawalAmount = await _freewillCustomerService.QueryWithdrawalBalance(accountCode!);
                var setTradeWithdrawalAmount = await _setTradeService.GetWithdrawalBalance(accountCode!);

                return freewillWithdrawalAmount <= setTradeWithdrawalAmount
                    ? freewillWithdrawalAmount
                    : setTradeWithdrawalAmount;
            case Product.Crypto:
            default:
                throw new NotImplementedException();
        }
    }

    public async Task VerifyUserGeBalance(
        string userId,
        string custCode,
        string currency,
        decimal requestWithdrawAmount
    )
    {
        var withdrawalAmountLimit = await GetAvailableWithdrawalAmount(userId, custCode, Product.GlobalEquities);

        if (withdrawalAmountLimit < requestWithdrawAmount)
        {
            throw new InvalidDataException($"Not enough balance");
        }
    }


    [Obsolete("This can produces incorrect result, Use another overload instead")]
    public async Task<BankAccountInfo> GetBankAccount(string userId, string customerCode)
    {
        var user = await _userService.GetUserInfoById(userId);
        if (!user.CustCodes.Contains(customerCode))
        {
            throw new InvalidDataException("Invalid cust code");
        }

        var bankAcc = await _freewillCustomerService.GetCustomerBankAccount(customerCode);
        if (bankAcc == null)
        {
            throw new InvalidDataException("Customer bank account not found");
        }

        var bankInfo = await _bankInfoService.GetByBankCode(bankAcc.BankCode);
        if (bankInfo == null)
        {
            throw new InvalidDataException("Bank info not found");
        }

        return new BankAccountInfo(
            bankAcc.BankAccountNo,
            bankInfo.Name,
            bankInfo.ShortName.ToUpperInvariant(),
            bankInfo.Code,
            bankInfo.IconUrl,
            bankInfo.Code == "025" ? "0001" : "0000" // hard code until data is ready on onboard-api
        );
    }

    public async Task<BankAccountInfo> GetBankAccount(string userId, string customerCode, Product product,
        TransactionType transactionType, User? user = null)
    {
        user ??= await _userService.GetUserInfoById(userId);

        if (!user.CustCodes.Contains(customerCode))
        {
            throw new InvalidDataException("Invalid cust code");
        }

        BankAccount? bankAccount = null;

        try
        {
            bankAccount = _featuresOptions.ShouldGetBankAccountFromOnboardService
                ? await _onboardService.GetBankAccount(customerCode, product, transactionType)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "OnboardService: Unable to get bank account with Exception: {Message}",
                ex.Message);
        }
        finally
        {
            bankAccount ??=
                await _freewillCustomerService.GetCustomerBankAccount(customerCode, product, transactionType);
        }

        if (bankAccount == null)
        {
            throw new NoBankAccountFoundException(
                $"Customer bank account not found for CustCode: {customerCode}, Product: {product.ToString()}, TransactionType: {transactionType.ToString()}");
        }

        var bankInfo = await _bankInfoService.GetByBankCode(bankAccount.BankCode);

        if (bankInfo == null)
        {
            throw new InvalidDataException("Bank info not found");
        }

        return new BankAccountInfo(
            bankAccount.BankAccountNo,
            bankInfo.Name,
            bankInfo.ShortName.ToUpperInvariant(),
            bankInfo.Code,
            bankInfo.IconUrl,
            bankAccount.BankBranchCode
        );
    }

    public async Task<AccountLimitValue> GetGeDepositLimit(string userId, string custCode, string currency)
    {
        try
        {
            const Product product = Product.GlobalEquities;
            var (user, _) = await GetAndValidateUserByProductType(userId, custCode, product);

            var accountCreditKycLimit = await _userService.GetCreditLimit(userId, custCode, product);

            var tradingPlatformAccount =
                await _globalTradeService.GetAccountSummary(user.GlobalAccount, currency);

            var unusedCash = decimal.Parse(tradingPlatformAccount.AvailableBalance, NumberStyles.Float);

            _logger.LogInformation(
                "accountCreditKycLimit: {AccountCreditKycLimit}, unusedCash: {UnusedCash}",
                accountCreditKycLimit, unusedCash);

            var depositLimit = decimal.Max(
                0,
                decimal.Round(accountCreditKycLimit - unusedCash, 2)
            );

            return new AccountLimitValue(
                decimal.Round(accountCreditKycLimit, 2),
                decimal.Round(unusedCash, 2),
                depositLimit
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WalletQueries: Unable to get deposit limit with Exception: {Message}",
                ex.Message);
            throw;
        }
    }

    public async Task<bool> GetAtsRegistrationStatus(string userId)
    {
        try
        {
            var userInfo = await _userService.GetUserInfoById(userId);
            var tradingAccounts =
                TradingAccountUtils.FindTradingAccountsByProduct(userInfo.ListTradingAccountNo, Product.CashBalance);
            if (tradingAccounts.Count == 0)
                throw new InvalidDataException($"Not found trading account: {tradingAccounts}");

            var tasks = new List<Task<ICustomerService.QueryAtsResponse>>();
            foreach (var tradingAccount in tradingAccounts)
            {
                tasks.Add(_freewillCustomerService.QueryAts(tradingAccount, "R"));
                tasks.Add(_freewillCustomerService.QueryAts(tradingAccount, "P"));
            }

            var responses = await Task.WhenAll(tasks);

            return Array.TrueForAll(responses,
                resp => resp.ResultCode == ResultCode._000 || resp.ResultCode == ResultCode._906);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WalletQueries: Unable to get Ats registration status with Exception: {Message}",
                ex.Message);
            return true;
        }
    }

    private async Task<(User user, string? accountCode)> GetAndValidateUserByProductType(string userId, string custCode,
        Product product)
    {
        var user = await _userService.GetUserInfoById(userId);
        if (!user.CustCodes.Contains(custCode))
        {
            throw new InvalidDataException("Invalid cust code");
        }

        var accountCode =
            TradingAccountUtils.FindTradingAccountByCustCodeAndProduct(
                custCode,
                user.ListTradingAccountNo,
                product
            );
        if (string.IsNullOrWhiteSpace(accountCode))
        {
            throw new InvalidDataException(
                $"Not found trading account {string.Join<string>(", ", user.ListTradingAccountNo)} with product: {product}");
        }

        return (user, accountCode);
    }

    public async Task<bool> CheckAtsAvailable(string userId, string customerCode, Product product)
    {
        try
        {
            if (product != Product.GlobalEquities)
            {
                return !_validationService.IsOutsideWorkingHour(product, Channel.ATS, DateUtils.GetThDateTimeNow(), out _);
            }

            // GE support only ODD deposit bank code
            var bankAccount = await GetBankAccount(userId, customerCode, product, TransactionType.Deposit);
            return _featuresOptions.OddDepositBankCode.Contains(bankAccount.Code);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("WalletQueries: Unable to check Ats availability with Exception: {Message}", ex.Message);
            return false;
        }
    }
}