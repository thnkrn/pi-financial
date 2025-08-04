using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class AccountBalance : Account, IAccountBalance
{
    public decimal TradingLimit { get; init; } = 100000m; //TODO after we integrate with onboarding srv
    public decimal WithdrawableCash { get; set; }
    public Currency Currency { get; set; }

    public decimal GetBalance(Provider provider, Currency currency)
    {
        if (provider != Provider.Velexa)
            throw new NotSupportedException(provider.ToString());

        if (currency != Currency.USD
            && currency != Currency.HKD
            && currency != Currency.THB)
            throw new NotSupportedException(currency.ToString());

        if (Currency == currency)
            return WithdrawableCash;

        throw new NotSupportedException(currency.ToString());
    }
}
