using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.AccountAggregate;

public class TradingAccountTest
{
    [Fact]
    public void Should_SetPositions_With_SameAccountNo_When_SetPositions()
    {
        // Arrange
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", "0800783-8", TradingAccountType.Cash);
        var positions = new List<AccountPosition>()
        {
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 100, Ttf.Nvdr, StockType.Normal),
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 100, Ttf.None, StockType.Lending),
            FakeAccountPosition(tradingAccount.AccountNo, "AOT", 100, Ttf.None, StockType.Normal),
            FakeAccountPosition("mismatched", "EA", 100, Ttf.None, StockType.Normal),
        };

        // Act
        tradingAccount.SetPositions(positions);

        // Assert
        Assert.Equal(3, tradingAccount.Positions?.Count());
        Assert.Equal(new[] { "EA", "EA", "AOT" }, tradingAccount.Positions!.Select(q => q.SecSymbol));
        Assert.Equal(new[] { StockType.Normal, StockType.Lending, StockType.Normal }, tradingAccount.Positions!.Select(q => q.StockType));
    }

    [Fact]
    public void Should_Return_ExpectedTotalVolume_When_GetTotalVolumeNvdrStock()
    {
        // Arrange
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", "0800783-8", TradingAccountType.Cash);
        var positions = new List<AccountPosition>()
        {
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 100.75m, Ttf.Nvdr, StockType.Normal),
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 500.78m, Ttf.Nvdr, StockType.Lending),
            FakeAccountPosition(tradingAccount.AccountNo, "AOT", 100, Ttf.None, StockType.Normal),
            FakeAccountPosition("mismatched", "EA", 100, Ttf.None, StockType.Normal),
        };
        tradingAccount.SetPositions(positions);

        // Act
        var actual = tradingAccount.GetTotalVolumeNvdrStock("EA");

        // Assert
        Assert.Equal(601.53m, actual);
    }

    [Theory]
    [InlineData(StockType.Normal, 100.75)]
    [InlineData(StockType.Lending, 500.78)]
    [InlineData(StockType.NewStockType82, 0)]
    public void Should_Return_ExpectedTotalVolume_When_GetTotalVolumeNoneNvdrStock(StockType stockType, decimal expected)
    {
        // Arrange
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", "0800783-8", TradingAccountType.Cash);
        var positions = new List<AccountPosition>()
        {
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 100.75m, Ttf.None, StockType.Normal),
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 200.75m, Ttf.Nvdr, StockType.Normal),
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 500.78m, Ttf.None, StockType.Lending),
            FakeAccountPosition(tradingAccount.AccountNo, "EA", 600.78m, Ttf.Nvdr, StockType.Lending),
            FakeAccountPosition(tradingAccount.AccountNo, "AOT", 100, Ttf.None, StockType.Normal),
            FakeAccountPosition("mismatched", "EA", 100, Ttf.None, StockType.Normal),
        };
        tradingAccount.SetPositions(positions);

        // Act
        var actual = tradingAccount.GetTotalVolumeNoneNvdrStock("EA", stockType);

        // Assert
        Assert.Equal(expected, actual);
    }

    private static AccountPosition FakeAccountPosition(string accountNo, string symbol, decimal avaiVolume, Ttf ttf, StockType stockType)
    {
        return new AccountPosition(symbol, ttf)
        {
            TradingAccountNo = "random",
            AccountNo = accountNo,
            StockType = stockType,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = avaiVolume,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0,
        };
    }
}
