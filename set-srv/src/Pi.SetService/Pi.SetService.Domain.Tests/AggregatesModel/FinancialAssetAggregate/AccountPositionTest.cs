using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.FinancialAssetAggregate;

public class AccountPositionTest
{
    [Theory]
    [InlineData(StockType.Short, OrderSide.Sell)]
    [InlineData(StockType.Borrow, OrderSide.Buy)]
    [InlineData(StockType.Normal, OrderSide.Buy)]
    [InlineData(StockType.Lending, OrderSide.Buy)]
    [InlineData(StockType.NewStockType8, OrderSide.Buy)]
    [InlineData(StockType.NewStockType82, OrderSide.Buy)]
    [InlineData(StockType.Unknow, OrderSide.Buy)]
    public void Should_ReturnExpectedOrderSide_When_Init_AccountPosition(StockType stockType, OrderSide expected)
    {
        // Act
        var position = new AccountPosition("EA", Ttf.None)
        {
            TradingAccountNo = "08001077-6",
            AccountNo = "080010776",
            StockType = stockType,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 0,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0,
        };

        // Assert
        Assert.Equal(expected, position.Side);
    }

    [Theory]
    [InlineData(StockType.Short, OrderAction.Short)]
    [InlineData(StockType.Borrow, OrderAction.Buy)]
    [InlineData(StockType.Normal, OrderAction.Buy)]
    [InlineData(StockType.Lending, OrderAction.Buy)]
    [InlineData(StockType.NewStockType8, OrderAction.Buy)]
    [InlineData(StockType.NewStockType82, OrderAction.Buy)]
    [InlineData(StockType.Unknow, OrderAction.Buy)]
    public void Should_ReturnExpectedOrderAction_When_Init_AccountPosition(StockType stockType, OrderAction expected)
    {
        // Act
        var position = new AccountPosition("EA", Ttf.None)
        {
            TradingAccountNo = "08001077-6",
            AccountNo = "080010776",
            StockType = stockType,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 0,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0,
        };

        // Assert
        Assert.Equal(expected, position.Action);
    }
}
