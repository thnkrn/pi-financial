using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class AccountSummaryTest
{
    static AccountSummary NewAccountSummary(
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
        decimal activeOrderCash = 0)
    {
        return new AccountSummary
        {
            Currency = currency,
            NetAssetValue = netAssetValue,
            TotalMarketValue = marketValue,
            TotalCost = cost,
            TotalUpnl = upnl,
            WithdrawableCash = withdrawalCash,
            LineAvailable = lineAvai,
            Positions = null
        };
    }

    public class TotalUpnlPercentage
    {
        [Fact]
        public void WhenCostEqualsZero_ReturnZero()
        {
            var value = 10000;
            var cost = 0;
            var accountSummary = NewAccountSummary(marketValue: value, upnl: 10000, cost: 0);

            Assert.Equal(0, accountSummary.TotalUpnlPercentage);
        }

        [Fact]
        public void WhenCostIsNotZero_ReturnUpnlPercentage()
        {
            var value = 250;
            var cost = 200;
            var accountSummary = NewAccountSummary(marketValue: value, cost: cost, upnl: 50);

            Assert.Equal(25m, accountSummary.TotalUpnlPercentage);
        }
    }
}
