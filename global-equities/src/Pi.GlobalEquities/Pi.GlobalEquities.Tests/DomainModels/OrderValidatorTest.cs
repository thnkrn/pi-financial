using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class OrderValidatorTest
{
    public class Validate_Test
    {
        [Fact]
        void MarketOrderWithStopAndLimitPrice_ReturnValidationResult()
        {
            var order = OrderTest.NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497", type: OrderType.Market, limitPrice: 200m, stopPrice: 300m);

            var result = OrderValidator.Validate(order);

            var expectedValue = new List<ValidationResult>
            {
                new("Limit price can not be set for market order"),
                new("Stop price can not be set for market order")
            };
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        void LimitOrderWithStopAndWithoutLimitPrice_ReturnValidationResult()
        {
            var order = OrderTest.NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497", type: OrderType.Limit, limitPrice: null, stopPrice: 300m);

            var result = OrderValidator.Validate(order);

            var expectedValue = new List<ValidationResult>
            {
                new("Limit price must be set for limit order"),
                new("Stop price can not be set for limit order")
            };
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Theory]
        [InlineData(121.999)]
        [InlineData(-1)]
        void LimitOrderWithIncorrectLimitPrice_ReturnValidationResult(decimal limit)
        {
            var order = OrderTest.NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497", type: OrderType.Limit, limitPrice: limit, stopPrice: null);

            var result = OrderValidator.Validate(order);

            var expectedValue = new List<ValidationResult>
            {
                new("The limit price is in an incorrect minor unit."),
            };
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        void StopLimitOrderWithOutLimitAndStopPrice_ReturnValidationResult()
        {
            var order = OrderTest.NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497", type: OrderType.StopLimit, limitPrice: null, stopPrice: null);

            var result = OrderValidator.Validate(order);

            var expectedValue = new List<ValidationResult>
            {
                new("Limit price must be set for stop limit order"),
                new("Stop price must be set for stop limit order")
            };
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Theory]
        [InlineData(121.999, 200.111)]
        [InlineData(-1, -1)]
        void StopLimitOrderWithIncorrectLimitAndStopPrice_ReturnValidationResult(decimal limit, decimal stop)
        {
            var order = OrderTest.NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497", type: OrderType.StopLimit, limitPrice: limit, stopPrice: stop);

            var result = OrderValidator.Validate(order);

            var expectedValue = new List<ValidationResult>
            {
                new("The limit price is in an incorrect minor unit."),
                new("The stop price is in an incorrect minor unit.")
            };
            result.Should().BeEquivalentTo(expectedValue);
        }
    }
}
