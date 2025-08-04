// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class TradeCalendarTypeTests
{
    public class ToTradeCalendarType_Test
    {
        [Theory]
        [InlineData("N", TradeCalendarType.BusinessDay)]
        [InlineData("E", TradeCalendarType.ExcludeDisallowedDay)]
        [InlineData("O", TradeCalendarType.FocusAllowedDay)]
        [InlineData("X", null)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void WhenValidShortCode_ReturnsExpectedTradeCalendarType(string shortCode, TradeCalendarType? expected)
        {
            // Act
            var result = shortCode.ToTradeCalendarType();

            // Assert
            result.Should().Be(expected);
        }
    }
}
