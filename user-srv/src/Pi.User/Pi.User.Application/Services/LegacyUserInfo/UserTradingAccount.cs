namespace Pi.User.Application.Services.LegacyUserInfo;

public record UserTradingAccount
{
    public required string CustomerCode { get; init; }
    public required IEnumerable<TradingAccount> TradingAccounts { get; init; }
}