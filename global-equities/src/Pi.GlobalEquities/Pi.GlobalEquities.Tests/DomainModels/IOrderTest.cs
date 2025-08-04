using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class IOrderTest
{
    public class CanBuy_Test
    {
        [Fact]
        void WhenOrderSideIsBuyAndOrderTypeIsMarket_ReturnTrue()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.Market, limitPrice: null, stopPrice: null, quantity: 0.02m);

            var canBuy = order.CanBuy(200);

            Assert.True(canBuy);
        }

        [Fact]
        void WhenOrderSideIsBuyAndOrderTypeIsLimitAndBalanceIsEnough_ReturnTrue()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.Limit, limitPrice: 100, stopPrice: null, quantity: 0.02m);

            var canBuy = order.CanBuy(200);

            Assert.True(canBuy);
        }

        [Fact]
        void WhenOrderSideIsBuyAndOrderTypeIsStopLimitAndBalanceIsEnough_ReturnTrue()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.StopLimit, limitPrice: 100, stopPrice: 120, quantity: 0.02m);

            var canBuy = order.CanBuy(200);

            Assert.True(canBuy);
        }

        [Fact]
        void WhenOrderSideIsBuyAndOrderTypeIsLimitAndBalanceIsNotEnough_ReturnFalse()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.Limit, limitPrice: 500, stopPrice: null, quantity: 0.02m);

            var canBuy = order.CanBuy(200);

            Assert.True(canBuy);
        }

        [Fact]
        void WhenOrderSideIsBuyAndOrderTypeIsStopLimitAndBalanceIsNotEnough_ReturnFalse()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.StopLimit, limitPrice: 500, stopPrice: 120, quantity: 0.02m);

            var canBuy = order.CanBuy(200);

            Assert.True(canBuy);
        }
    }

    public class GetFilledQuantity_Test
    {
        [Fact]
        void WhenFillIsEmpty_ReturnsZero()
        {
            var order = (IOrder)OrderTest.NewOrder(fills: Enumerable.Empty<OrderFill>());

            var filledQuantity = order.GetFilledQuantity();

            Assert.Equal(0, filledQuantity);
        }

        [Fact]
        void WhenThereAreFills_ReturnsZero()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);

            var order = (IOrder)OrderTest.NewOrder(fills: new List<OrderFill> { fill1, fill2 });

            var filledQuantity = order.GetFilledQuantity();

            var expectFilledQuantity = fill1.Quantity + fill2.Quantity;
            Assert.Equal(expectFilledQuantity, filledQuantity);
        }
    }

    public class GetCancelledQuantity_Test
    {
        [Fact]
        void WhenFillIsEmpty_ReturnsZero()
        {
            var order = (IOrder)OrderTest.NewOrder(fills: Enumerable.Empty<OrderFill>());

            var filledQuantity = order.GetFilledQuantity();

            Assert.Equal(0, filledQuantity);
        }

        [Fact]
        void WhenThereAreFills_ReturnsZero()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);

            var order = (IOrder)OrderTest.NewOrder(fills: new List<OrderFill> { fill1, fill2 });

            var filledQuantity = order.GetFilledQuantity();

            var expectFilledQuantity = fill1.Quantity + fill2.Quantity;
            Assert.Equal(expectFilledQuantity, filledQuantity);
        }
    }

    public class GetFilledPrice_Test
    {
        [Fact]
        void WhenThereIsNoFills_ReturnZero()
        {
            var order = (IOrder)OrderTest.NewOrder(fills: Enumerable.Empty<OrderFill>());

            var result = order.GetFilledPrice();

            Assert.Equal(0, result);
        }

        [Fact]
        void WhenThereAreFills_ReturnFiiledPrice()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);

            var order = (IOrder)OrderTest.NewOrder(fills: new List<OrderFill> { fill1, fill2 });

            var result = order.GetFilledPrice();

            var expectedFilledPrice = (fill1.Price * fill1.Quantity + fill2.Price * fill2.Quantity) /
                                      (fill1.Quantity + fill2.Quantity);
            Assert.Equal(expectedFilledPrice, result);
        }
    }

    public class GetFilledCost_Test
    {
        [Fact]
        void WhenThereIsNoFills_ReturnZero()
        {
            var order = (IOrder)OrderTest.NewOrder(fills: Enumerable.Empty<OrderFill>());

            var result = order.GetFilledCost();

            Assert.Equal(0, result);
        }

        [Fact]
        void WhenThereAreFills_ReturnFiiledPrice()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);

            var order = (IOrder)OrderTest.NewOrder(fills: new List<OrderFill> { fill1, fill2 });

            var result = order.GetFilledCost();

            var expectedFilledCost = fill1.Price * fill1.Quantity + fill2.Price * fill2.Quantity;
            Assert.Equal(expectedFilledCost, result);
        }
    }

    public class GetActiveCash_Test
    {
        [Fact]
        void WhenSideIsNotBuy_ReturnZero()
        {
            var order = (IOrder)OrderTest.NewOrder(side: OrderSide.Sell);

            var result = order.GetActiveCash();

            Assert.Equal(0, result);
        }

        [Fact]
        void WhenLimitOrderSideIsBuy_ReturnActiveCash()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.Limit, quantity: 0.02m, limitPrice: 121, stopPrice: null, side: OrderSide.Buy);

            var result = order.GetActiveCash();

            var expectedResult = order.Quantity * order.LimitPrice;
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        void WhenLimitPriceIsNullAndSideIsBuy_ReturnZero()
        {
            var order = (IOrder)OrderTest.NewOrder(type: OrderType.Market, quantity: 0.02m, limitPrice: null, stopPrice: null, side: OrderSide.Buy);

            var result = order.GetActiveCash();

            Assert.Equal(0, result);
        }
    }
}
