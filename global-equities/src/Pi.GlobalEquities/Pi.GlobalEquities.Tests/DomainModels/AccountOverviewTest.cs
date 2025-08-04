using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class AccountOverviewTest
{
    static AccountOverview NewMockAccountOverview(
        string accountId = null,
        string tradingAccNoDis = null,
        string tradingAccNo = null,
        decimal tradingLim = 0,
        decimal withdrawalCash = 0,
        decimal lineAvai = 0,
        Currency currency = Currency.USD,
        decimal netAssetValue = 0,
        decimal marketValue = 0,
        decimal cost = 0,
        decimal upnl = 0,
        decimal activeOrderCash = 0
        )
    {
        return new AccountOverview(currency, netAssetValue, marketValue, cost, upnl, activeOrderCash)
        {
            AccountId = accountId,
            TradingAccountNoDisplay = tradingAccNoDis,
            TradingAccountNo = tradingAccNo,
            TradingLimit = tradingLim,
            WithdrawableCash = withdrawalCash,
            LineAvailable = lineAvai
        };
    }

    public class ChangeCurrency_Test
    {
        [Fact]
        public void WhenFromExchangeRateIsNotMatchedWithCurrentCurrency_ThrowArgumentException()
        {
            var value = 10000;
            var accountBalance = NewMockAccountOverview(currency: Currency.USD);
            var exRate = new ExchangeRate { From = Currency.HKD, To = Currency.THB, Rate = 4.44m };

            var action = () => accountBalance.ChangeCurrency(exRate);

            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal("Current currency is not valid to exchange", exception.Message);
        }

        [Fact]
        public void WhenFromExchangeRateIsMatchedWithCurrentCurrency_ReturnCorrectValue()
        {
            var exRate = new ExchangeRate { From = Currency.USD, To = Currency.THB, Rate = 34.30m };
            var value = 10000;
            var accountBalance = NewMockAccountOverview(
                tradingLim: value,
                withdrawalCash: value,
                lineAvai: value,
                netAssetValue: value,
                marketValue: value,
                cost: value,
                upnl: 1,
                activeOrderCash: value
                );

            accountBalance.ChangeCurrency(exRate);

            var expectedValue = 343000;
            Assert.Equal(Currency.THB, accountBalance.Currency);
            Assert.Equal(expectedValue, accountBalance.TradingLimit);
            Assert.Equal(expectedValue, accountBalance.WithdrawableCash);
            Assert.Equal(expectedValue, accountBalance.LineAvailable);
            Assert.Equal(expectedValue, accountBalance.NetAssetValue);
            Assert.Equal(expectedValue, accountBalance.MarketValue);
            Assert.Equal(expectedValue, accountBalance.Cost);
            Assert.Equal(expectedValue, accountBalance.ActiveOrderCash);
        }
    }

    public class UpnlPercentageTest
    {
        [Fact]
        public void WhenCostEqualsZero_ReturnZero()
        {
            var accountBalance = NewMockAccountOverview(cost: 0);

            Assert.Equal(0, accountBalance.UpnlPercentage);
        }

        [Fact]
        public void WhenCostIsNotZero_ReturnUpnlPercentage()
        {
            var value = 250;
            var cost = 200;
            var accountBalance = NewMockAccountOverview(marketValue: value, cost: cost, upnl: 50);

            Assert.Equal(25m, accountBalance.UpnlPercentage);
        }
    }
}
