// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class FundTradeDataTests
{
    public class GetTradableDates_Test
    {
        [Fact]
        public void WhenBusinessDay_ReturnsBusinessDaysExcludingHolidays()
        {
            // Arrange
            var fundTradeData = new FundTradeData
            {
                Holidays = new List<DateTime> { new DateTime(2024, 12, 25) } // Christmas
            };
            var businessDays = new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 25),
                    new DateTime(2024, 12, 26),
                    new DateTime(2024, 12, 27)
                };

            // Act
            var result = fundTradeData.GetTradableDates(TradeSide.Buy, TradeCalendarType.BusinessDay, businessDays);

            // Assert
            result.Should().BeEquivalentTo(new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 26),
                    new DateTime(2024, 12, 27)
                });
        }

        [Fact]
        public void WhenFocusAllowedDay_ReturnsAllowedDaysIntersectingBusinessDaysExcludingHolidays()
        {
            // Arrange
            var fundTradeData = new FundTradeData
            {
                Holidays = new List<DateTime> { new DateTime(2024, 12, 25) }, // Christmas
                TradeCalendars = new List<TradeCalendar>
                    {
                        new TradeCalendar { TradeDate = new DateTime(2024, 12, 24), TransactionCode = "SUB", TradePermission = TradePermission.Allowed },
                        new TradeCalendar { TradeDate = new DateTime(2024, 12, 26), TransactionCode = "SUB", TradePermission = TradePermission.Allowed }
                    }
            };
            var businessDays = new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 25),
                    new DateTime(2024, 12, 26),
                    new DateTime(2024, 12, 27)
                };

            // Act
            var result = fundTradeData.GetTradableDates(TradeSide.Buy, TradeCalendarType.FocusAllowedDay, businessDays);

            // Assert
            result.Should().BeEquivalentTo(new List<DateTime>
                {
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 26)
                });
        }

        [Fact]
        public void WhenExcludeDisallowedDay_ReturnsBusinessDaysExcludingHolidaysAndDisallowedDays()
        {
            // Arrange
            var fundTradeData = new FundTradeData
            {
                Holidays = new List<DateTime> { new DateTime(2024, 12, 25) }, // Christmas
                TradeCalendars = new List<TradeCalendar>
                    {
                        new TradeCalendar { TradeDate = new DateTime(2024, 12, 24), TransactionCode = "SUB", TradePermission = TradePermission.Disallowed },
                        new TradeCalendar { TradeDate = new DateTime(2024, 12, 26), TransactionCode = "SUB", TradePermission = TradePermission.Disallowed }
                    }
            };
            var businessDays = new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 25),
                    new DateTime(2024, 12, 26),
                    new DateTime(2024, 12, 27)
                };

            // Act
            var result = fundTradeData.GetTradableDates(TradeSide.Buy, TradeCalendarType.ExcludeDisallowedDay, businessDays);

            // Assert
            result.Should().BeEquivalentTo(new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 27)
                });
        }

        [Fact]
        public void WhenInvalidTradeCalendarType_ReturnsEmpty()
        {
            // Arrange
            var fundTradeData = new FundTradeData();
            var businessDays = new List<DateTime>
                {
                    new DateTime(2024, 12, 23),
                    new DateTime(2024, 12, 24),
                    new DateTime(2024, 12, 25),
                    new DateTime(2024, 12, 26),
                    new DateTime(2024, 12, 27)
                };

            // Act
            var result = fundTradeData.GetTradableDates(TradeSide.Buy, null, businessDays);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
