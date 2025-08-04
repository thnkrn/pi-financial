using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
public class TradingAccount
{
    public TradingAccount(
        string customerCode,
        string tradingAccountNo,
        string accountTypeCode,
        string exchangeMarketId,
        string? marketingId,
        string accountStatus)
    {
        CustomerCode = customerCode.Trim();
        TradingAccountNo = tradingAccountNo.Trim();
        AccountTypeCode = accountTypeCode;
        ExchangeMarketId = exchangeMarketId;
        AccountStatus = accountStatus;
        MarketingId = marketingId?.Trim();
        CreditLineCurrency = "THB";
    }

    public string CustomerCode { get; private set; }
    public string TradingAccountNo { get; private set; }
    public string AccountTypeCode { get; private set; }
    public string ExchangeMarketId { get; private set; }
    public string? AccountStatus { get; private set; }
    public decimal CreditLine { get; private set; }
    public string CreditLineCurrency { get; private set; }
    public DateTime? CreditLineEffectiveDate { get; private set; }
    public DateTime? CreditLineEndDate { get; private set; }
    public string? MarketingId { get; private set; }
    public DateTime? AccountOpeningDate { get; private set; }
    public TradingAccount WithCreditLine(decimal creditLine, string currency, DateTime? effectiveDate, DateTime? endDate)
    {
        CreditLine = creditLine;
        CreditLineCurrency = currency;
        CreditLineEffectiveDate = effectiveDate;
        CreditLineEndDate = endDate;
        return this;
    }
}