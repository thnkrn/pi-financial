using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Tests.Models;

public class AccountSblInstrumentBalanceTest
{
    [Theory]
    [InlineData(100000, 5000, 57.50, 100)]
    [InlineData(100000, 8000, 57.50, 100)]
    [InlineData(10000, 900000, 57.50, 10000)]
    [InlineData(10000, 900000, 0, 0)]
    public void Should_Return_Expected_When_Init(decimal availableLending, decimal ee, decimal closePrice, decimal expected)
    {
        // Arrange
        var symbol = "BBL";
        var tradingAccountNo = "0803177-6";
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = ee,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "",
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = closePrice
        };
        var sblInstrument = new SblInstrumentInfo
        {
            SblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, availableLending),
            CorporateActions = [],
            MarginInfo = new EquityMarginInfo(symbol, 70m, false),
        };

        // Act
        var result = new AccountSblInstrumentBalance(symbol, accountBalance, [position], tradingDetail, sblInstrument);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.SblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.True(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(sblInstrument.MarginInfo.Rate, result.MarginRate);
        Assert.Equal(expected, result.MaximumShares);
    }

    [Fact]
    public void Should_Return_Expected_When_PositionIsEmpty()
    {
        // Arrange
        var symbol = "BBL";
        var tradingAccountNo = "0803177-6";
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 9000000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var sblInstrument = new SblInstrumentInfo
        {
            SblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, 100000000),
            CorporateActions = [],
            MarginInfo = new EquityMarginInfo(symbol, 70m, false),
        };

        // Act
        var result = new AccountSblInstrumentBalance(symbol, accountBalance, [], tradingDetail, sblInstrument);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.SblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.True(result.AllowBorrowing);
        Assert.Equal(0, result.ShortUnit);
        Assert.Equal(sblInstrument.MarginInfo.Rate, result.MarginRate);
        Assert.Equal(sblInstrument.SblInstrument.AvailableLending, result.MaximumShares);
    }

    [Fact]
    public void Should_Return_Expected_When_CorporateActionIsNotEmpty()
    {
        // Arrange
        var symbol = "BBL";
        var tradingAccountNo = "0803177-6";
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 9000000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "",
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };
        var sblInstrument = new SblInstrumentInfo
        {
            SblInstrument = new SblInstrument(Guid.NewGuid(), symbol, 5.00m, 2000000, 1000000, 100000000),
            CorporateActions = [
                new CorporateAction
                {
                    Date = default,
                    CaType = "XD"
                }
            ],
            MarginInfo = new EquityMarginInfo(symbol, 70m, false),
        };

        // Act
        var result = new AccountSblInstrumentBalance(symbol, accountBalance, [position], tradingDetail, sblInstrument);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.True(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(sblInstrument.SblInstrument.AvailableLending, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.False(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(sblInstrument.MarginInfo.Rate, result.MarginRate);
        Assert.Equal(sblInstrument.SblInstrument.AvailableLending, result.MaximumShares);
    }

    [Fact]
    public void Should_Return_Expected_When_SblInstrumentInfoIsEmpty()
    {
        // Arrange
        var symbol = "BBL";
        var tradingAccountNo = "0803177-6";
        var accountBalance = new AvailableCreditBalance
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "random",
            TraderId = "909",
            CreditLimit = 100001,
            BuyCredit = 100002,
            CashBalance = 100003,
            Ar = 0,
            Ap = 0,
            Liability = 0,
            Asset = 0,
            Equity = 1000,
            MarginRequired = 0,
            ExcessEquity = 9000000000,
            CallForce = 0,
            CallMargin = 0,
            Pp = 2000
        };
        var position = new AccountPositionCreditBalance(symbol, Ttf.None)
        {
            TradingAccountNo = tradingAccountNo,
            AccountNo = "",
            StockType = StockType.Short,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 100,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var tradingDetail = new TradingDetail
        {
            Price = 10,
            PrevClose = 9.5m
        };

        // Act
        var result = new AccountSblInstrumentBalance(symbol, accountBalance, [position], tradingDetail, null);

        // Assert
        Assert.Equal(symbol, result.Symbol);
        Assert.False(result.SblEnabled);
        Assert.Equal(accountBalance.ExcessEquity, result.ExcessEquity);
        Assert.Equal(accountBalance.Pp, result.PurchesingPower);
        Assert.Equal(0, result.AvailableLending);
        Assert.Equal(tradingDetail.PrevClose, result.ClosePrice);
        Assert.False(result.AllowBorrowing);
        Assert.Equal(100, result.ShortUnit);
        Assert.Equal(0, result.MarginRate);
        Assert.Equal(0, result.MaximumShares);
    }
}
