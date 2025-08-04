// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class BusinessCalendarTests
{
    public class GetBusinessDays_Test
    {
        [Fact]
        public void WhenNoHolidaysAndWeekends_ReturnsAllBusinessDays()
        {
            // Arrange
            var businessCalendar = new BusinessCalendar
            {
                BusinessHolidays = new List<DateTime>()
            };
            var start = new DateTime(2024, 12, 23); // Monday
            var end = new DateTime(2024, 12, 27); // Friday

            // Act
            var result = businessCalendar.GetBusinessDays(start, end);

            // Assert
            result.Should().BeEquivalentTo(new List<DateTime>
            {
                new DateTime(2024, 12, 23),
                new DateTime(2024, 12, 24),
                new DateTime(2024, 12, 25),
                new DateTime(2024, 12, 26),
                new DateTime(2024, 12, 27)
            });
        }

        [Fact]
        public void WhenHolidaysAndWeekends_ReturnsOnlyBusinessDays()
        {
            // Arrange
            var businessCalendar = new BusinessCalendar
            {
                BusinessHolidays = new List<DateTime>
                {
                    new DateTime(2024, 12, 25) // Christmas
                }
            };
            var start = new DateTime(2024, 12, 23); // Monday
            var end = new DateTime(2024, 12, 27); // Friday

            // Act
            var result = businessCalendar.GetBusinessDays(start, end);

            // Assert
            result.Should().BeEquivalentTo(new List<DateTime>
            {
                new DateTime(2024, 12, 23),
                new DateTime(2024, 12, 24),
                new DateTime(2024, 12, 26),
                new DateTime(2024, 12, 27)
            });
        }
    }
}
