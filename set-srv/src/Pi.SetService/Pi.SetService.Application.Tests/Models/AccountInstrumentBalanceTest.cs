using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Tests.Models;

public class AccountInstrumentBalanceTest
{
    [Fact]
    public void Should_Return_Expected_When_InitClassSuccess_With_CashBalance()
    {
        // Arrange
        var symbol = "AAPL";
        var availableBalance = new AvailableCashBalance
        {
            CashBalance = 500m,
            Ar = 0,
            Ap = 0,
            ArTrade = 0,
            ApTrade = 0,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0,
            TradingAccountNo = "",
            AccountNo = "",
            TraderId = "",
            CreditLimit = 0,
            BuyCredit = 600m
        };
        var assets = new List<AccountPosition>()
        {
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 10000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.Nvdr)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 20000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Short,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 30000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.Nvdr)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Short,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 40000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 1,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new("mismatched", Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 10000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
        };

        // Act
        var actual = new AccountInstrumentBalance(symbol, availableBalance, assets);

        // Assert
        Assert.Equal(availableBalance.BuyCredit, actual.Balance);
        Assert.Equal("THB", actual.BalanceUnit);
        Assert.Equal(10001m, actual.Unit);
        Assert.Equal(20000m, actual.NvdrUnit);
        Assert.Equal(30000m, actual.ShortUnit);
        Assert.Equal(40000m, actual.ShortNvdrUnit);
    }

    [Fact]
    public void Should_Return_Expected_When_InitClassSuccess_With_CreditBalance()
    {
        // Arrange
        var symbol = "AAPL";
        var availableBalance = new AvailableCreditBalance
        {
            CashBalance = 500m,
            Ar = 0,
            Ap = 0,
            TradingAccountNo = "",
            AccountNo = "",
            TraderId = "",
            CreditLimit = 0,
            BuyCredit = 0,
            Liability = 0,
            Asset = 0,
            Equity = 0,
            MarginRequired = 0,
            ExcessEquity = 50000m,
            CallForce = 0,
            CallMargin = 0
        };
        var assets = new List<AccountPosition>()
        {
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 10000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.Nvdr)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 20000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Short,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 30000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.Nvdr)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Short,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 40000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(symbol, Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 1,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new("mismatched", Ttf.None)
            {
                TradingAccountNo = "",
                AccountNo = "",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 10000,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
        };

        // Act
        var actual = new AccountInstrumentBalance(symbol, availableBalance, assets);

        // Assert
        Assert.Equal(availableBalance.ExcessEquity, actual.Balance);
        Assert.Equal("THB", actual.BalanceUnit);
        Assert.Equal(10001m, actual.Unit);
        Assert.Equal(20000m, actual.NvdrUnit);
        Assert.Equal(30000m, actual.ShortUnit);
        Assert.Equal(40000m, actual.ShortNvdrUnit);
    }
}
