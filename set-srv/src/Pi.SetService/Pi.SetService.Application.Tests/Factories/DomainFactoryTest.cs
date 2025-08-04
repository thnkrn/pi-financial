using Pi.SetService.Application.Factories;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Tests.Factories;

public class DomainFactoryTest
{
    [Theory]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.ShortCover)]
    [InlineData(OrderAction.Buy, OrderSide.Buy, OrderType.Normal)]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.ShortCover)]
    [InlineData(OrderAction.Sell, OrderSide.Sell, OrderType.Normal)]
    public void Should_Return_Expected_OrderSideAndOrderType_When_NewOrderSideAndOrderType(OrderAction action, OrderSide side, OrderType type)
    {
        // Act
        var (actualSide, actualType) = DomainFactory.NewOrderSideAndOrderType(action, null);

        // Assert
        Assert.Equal(side, actualSide);
        Assert.Equal(type, actualType);
    }

    [Theory]
    [InlineData(OrderAction.Cover, true, OrderSide.Buy, OrderType.ShortCover)]
    [InlineData(OrderAction.Cover, false, OrderSide.Buy, OrderType.ShortCover)]
    [InlineData(OrderAction.Buy, true, OrderSide.Buy, OrderType.Normal)]
    [InlineData(OrderAction.Buy, false, OrderSide.Buy, OrderType.Normal)]
    [InlineData(OrderAction.Short, true, OrderSide.Sell, OrderType.ShortCover)]
    [InlineData(OrderAction.Short, false, OrderSide.Sell, OrderType.ShortCover)]
    [InlineData(OrderAction.Sell, true, OrderSide.Sell, OrderType.SellLending)]
    [InlineData(OrderAction.Sell, false, OrderSide.Sell, OrderType.Normal)]
    public void Should_Return_Expected_OrderSideAndOrderType_When_NewOrderSideAndOrderType_With_Lending(OrderAction action, bool lending, OrderSide side, OrderType type)
    {
        // Act
        var (actualSide, actualType) = DomainFactory.NewOrderSideAndOrderType(action, lending);

        // Assert
        Assert.Equal(side, actualSide);
        Assert.Equal(type, actualType);
    }
}
