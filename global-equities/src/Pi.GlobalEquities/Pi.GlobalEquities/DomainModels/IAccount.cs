using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public interface IAccount
{
    string Id { get; }
    string UserId { get; }
    string CustCode { get; }
    string TradingAccountNo { get; }
    public bool EnableBuy { get; }
    public bool EnableSell { get; }
    DateTime UpdatedAt { get; }
    string GetProviderAccount(Provider provider);
    bool IsExpired();
}

public interface IAccountBalance : IAccount
{
    decimal GetBalance(Provider provider, Currency currency);
}
