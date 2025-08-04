using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Common.Utilities;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Freewill.Model;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using BankAccountInfoItem = Pi.User.Application.Services.LegacyUserInfo.BankAccountInfoItem;

namespace Pi.User.Infrastructure.Services;

public class FreewillCustomerService : IUserBankAccountService, IUserTradingAccountService
{
    private const string DateFormat = "yyyyMMdd";
    private const string TimeFormat = "HH:mm:ss";
    private readonly ICustomerModuleApi _customerModuleApi;
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly ILogger<FreewillCustomerService> _logger;
    private readonly ITransactionIdRepository _transactionIdRepository;

    public FreewillCustomerService(
        ICustomerModuleApi customerModuleApi,
        DateTimeProvider dateTimeProvider,
        ILogger<FreewillCustomerService> logger,
        ITransactionIdRepository transactionIdRepository)
    {
        _customerModuleApi = customerModuleApi;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _transactionIdRepository = transactionIdRepository;
    }

    public async Task<BankAccountInfo?> GetBankAccountInfoAsync(string customerCode,
        CancellationToken cancellationToken = default)
    {
        var dateTime = GetBkkDateTime();
        var request = new GetBankAccInfoRequest($"QB{dateTime:yyyyMMddHHmmss}",
            dateTime.ToString(DateFormat, CultureInfo.InvariantCulture),
            dateTime.ToString(TimeFormat, CultureInfo.InvariantCulture), customerCode);

        var response = await _customerModuleApi.QueryCustomerBankAccountInfoAsync(request, cancellationToken);
        if (response.ResultCode == ResultCode._000)
        {
            if (response.ResultList is null) return null;
            return new BankAccountInfo(customerCode, response.ResultList.Select(MapToBankAccountInfoItem).ToList(),
                decimal.ToInt32(response.ResultListTotal));
        }

        throw new Exception(
            $"[{customerCode}][{request.ReferId}] Freewill returns non-success result code: {response.ResultCode} {response.Reason}");
    }

    public async Task UpdateBankAccountInfoAsync(BankAccountInfo bankAccountInfo,
        CancellationToken cancellationToken = default)
    {
        var dateTime = GetBkkDateTime();
        var sendDate = dateTime.ToString(DateFormat, CultureInfo.InvariantCulture);
        var referId = $"UB{dateTime:yyyyMMddHHmmss}";
        var transactionId = await _transactionIdRepository.GetNextAsync("UB", DateOnly.FromDateTime(dateTime.Date),
            referId, bankAccountInfo.CustomerCode, cancellationToken);
        await _transactionIdRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        var request = new UpdateBankAccountInfoRequest(
            referId,
            sendDate,
            dateTime.ToString(TimeFormat, CultureInfo.InvariantCulture),
            bankAccountInfo.CustomerCode,
            transactionId.ToString(),
            bankAccountInfo.Items.Select(MapToFreewillBankAccountInfoItem).ToList());

        _logger.LogInformation("[{CustomerCode}][{RequestReferId}] Updating bank account info",
            bankAccountInfo.CustomerCode, request.ReferId);
        var response = await _customerModuleApi.UpdateBankAccountInfoAsync(request, cancellationToken);

        if (response.ResultCode != ResultCode._000 && response.ResultCode != ResultCode._001)
            throw new Exception(
                $"[{bankAccountInfo.CustomerCode}][{request.ReferId}] Freewill returns non-success result code: {response.ResultCode} {response.Reason}");
    }

    public async Task<UserTradingAccount> GetUserTradingAccountByCustomerCodeAsync(string customerCode,
        CancellationToken cancellationToken = default)
    {
        var dateTime = GetBkkDateTime();
        var request = new GetAccountInfoRequest($"QI{dateTime:yyyyMMddHHmmss}",
            dateTime.ToString(DateFormat, CultureInfo.InvariantCulture),
            dateTime.ToString(TimeFormat, CultureInfo.InvariantCulture), customerCode);
        var response = await _customerModuleApi.QueryCustomerAccountInfoAsync(request, cancellationToken);

        if (response.ResultCode == ResultCode._000)
        {
            if (response.AccountList == null)
                return new UserTradingAccount
                {
                    CustomerCode = customerCode,
                    TradingAccounts = Enumerable.Empty<TradingAccount>()
                };

            return new UserTradingAccount
            {
                CustomerCode = customerCode,
                TradingAccounts = response.AccountList.Select(o => new TradingAccount
                {
                    TradingAccountNo = o.Account,
                    AccountStatus = o.Acctstatus,
                    CreditLine = decimal.Parse(o.Appcreditline, NumberStyles.Float, CultureInfo.InvariantCulture),
                    CreditLineEffectiveDate = MapDate(o.Lineeffective),
                    CreditLineEndDate = MapDate(o.Lineexpire),
                    MarketingId = o.Mktid,
                    AccountType = o.Custacct,
                    AccountTypeCode = o.Acctcode,
                    AccountOpeningDate = MapDate(o.Opendate),
                    ExchangeMarketId = o.Xchgmkt
                })
            };
        }

        throw new Exception(
            $"[{customerCode}][{request.ReferId}] Freewill returns non-success result code: {response.ResultCode} {response.Reason}");
    }

    private UpdateBankAccountInfoRequestAllOfDetailListInner MapToFreewillBankAccountInfoItem(BankAccountInfoItem src)
    {
        return new UpdateBankAccountInfoRequestAllOfDetailListInner(
            src.TradingAccountNo,
            acctcode: src.AccountCodeType,
            bankaccno: src.BankAccountNo,
            bankacctype: src.BankAccountType,
            bankcode: src.BankCode,
            custacct: src.AccountType,
            effdate: src.EffectiveDate.HasValue
                ? src.EffectiveDate!.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
                : string.Empty,
            enddate: src.EndDate.HasValue
                ? src.EndDate!.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
                : string.Empty,
            paytype: src.PayType,
            rptype: src.RPType,
            transtype: src.TransactionType);
    }

    private BankAccountInfoItem MapToBankAccountInfoItem(GetBankAccInfoResponseItem src)
    {
        var effectiveDate = MapDate(src.Effdate);
        var endDate = MapDate(src.Enddate);
        return new BankAccountInfoItem(
            src.Account,
            src.Custacct,
            src.Transtype,
            src.Rptype,
            src.Bankcode,
            src.Bankaccno,
            src.Bankacctype,
            src.Paytype,
            effectiveDate,
            endDate,
            src.Acctcode,
            src.Xchgmkt);
    }

    private static DateOnly? MapDate(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;

        if (DateOnly.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var result))
            return result;
        return DateOnly.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
    }

    private DateTimeOffset GetBkkDateTime()
    {
        var dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(_dateTimeProvider.OffsetNow(), "Asia/Bangkok");
        return dateTime;
    }
}