// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class FundamentalTests
{
    public class GetTradeCalendarType_Test
    {
        [Theory]
        [InlineData(TradeSide.Buy, TradeCalendarType.BusinessDay)]
        [InlineData(TradeSide.Sell, TradeCalendarType.ExcludeDisallowedDay)]
        [InlineData(TradeSide.Switch, TradeCalendarType.FocusAllowedDay)]
        public void WhenValidTradeSide_ReturnsExpectedTradeCalendarType(TradeSide tradeSide, TradeCalendarType? expected)
        {
            // Arrange
            var fundamental = new Fundamental
            {
                BuyCalendarType = TradeCalendarType.BusinessDay,
                SellCalendarType = TradeCalendarType.ExcludeDisallowedDay,
                SwitchOutCalendarType = TradeCalendarType.FocusAllowedDay
            };

            // Act
            var result = fundamental.GetTradeCalendarType(tradeSide);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void WhenInvalidTradeSide_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var fundamental = new Fundamental();
            var invalidTradeSide = (TradeSide)999;

            // Act
            Action act = () => fundamental.GetTradeCalendarType(invalidTradeSide);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Not expected TradeSide value: 999. (Parameter 'tradeSide')");
        }
    }
}
