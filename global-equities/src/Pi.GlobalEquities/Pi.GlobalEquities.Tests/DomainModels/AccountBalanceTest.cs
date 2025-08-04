using FluentAssertions;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class AccountBalanceTest
{
    static AccountBalance NewMockAccountBalance(
        string id = null,
        string userId = null,
        string custCode = null,
        string TradingAccNo = null,
        string velexaAcc = null,
        DateTime updateTime = new(),
        decimal tradeLimit = 0m,
        decimal withdrawalCash = 0m,
        Currency currency = new()
        )
    {
        return new AccountBalance
        {
            Id = id,
            UserId = userId,
            CustCode = custCode,
            TradingAccountNo = TradingAccNo,
            VelexaAccount = velexaAcc,
            UpdatedAt = updateTime,
            TradingLimit = tradeLimit,
            WithdrawableCash = withdrawalCash,
            Currency = currency
        };
    }

    public class GetBalance_Test
    {
        [Fact]
        public void WhenProviderAccountIsNotVelexa_ThrowNotSupportedException()
        {
            var accBalance = NewMockAccountBalance();
            var notVelexaProv = (Provider)0;

            var action = () => accBalance.GetBalance(notVelexaProv, Currency.USD);

            var exception = action.Should().Throw<NotSupportedException>().Which;

            Assert.Equal(notVelexaProv.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(Currency.AUD)]
        [InlineData(Currency.JPY)]
        [InlineData(Currency.NZD)]
        public void WhenCurrencyIsNotSupport_ThrowNotSupportedException(Currency currency)
        {
            var accBalance = NewMockAccountBalance();

            var action = () => accBalance.GetBalance(Provider.Velexa, currency);

            var exception = action.Should().Throw<NotSupportedException>().Which;

            Assert.Equal(currency.ToString(), exception.Message);
        }

        [Fact]
        public void WhenCurrencyIsNotEqualBalanceCurrency_ThrowNotSupportedException()
        {
            var accountCurrency = Currency.USD;
            var expectedCurrency = Currency.HKD;
            var accBalance = NewMockAccountBalance(currency: accountCurrency);

            var action = () => accBalance.GetBalance(Provider.Velexa, expectedCurrency);

            var exception = action.Should().Throw<NotSupportedException>().Which;

            Assert.Equal(Currency.HKD.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(Provider.Velexa, Currency.USD)]
        [InlineData(Provider.Velexa, Currency.HKD)]
        [InlineData(Provider.Velexa, Currency.THB)]
        public void WhenProviderAccountIsVelexaCurrencyIsSupported_ReturnWithdrawalBalance(Provider provider, Currency currency)
        {
            var withdrawalBal = 10000;
            var accBalance = NewMockAccountBalance(withdrawalCash: withdrawalBal, currency: currency);

            var withdrawalBalanceRes = accBalance.GetBalance(provider, currency);
            Assert.Equal(withdrawalBal, withdrawalBalanceRes);
        }
    }
}
