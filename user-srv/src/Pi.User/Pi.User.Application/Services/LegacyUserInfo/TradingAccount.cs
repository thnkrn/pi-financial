namespace Pi.User.Application.Services.LegacyUserInfo;

public record TradingAccount
{
    public required string TradingAccountNo { get; init; }
    public required string AccountType { get; init; }
    public required string ExchangeMarketId { get; init; }
    public required string AccountTypeCode { get; init; }
    public required string AccountStatus { get; init; }
    public decimal CreditLine { get; init; }
    public DateOnly? CreditLineEffectiveDate { get; init; }
    public DateOnly? CreditLineEndDate { get; init; }
    public string? MarketingId { get; init; }
    public DateOnly? AccountOpeningDate { get; init; }
};