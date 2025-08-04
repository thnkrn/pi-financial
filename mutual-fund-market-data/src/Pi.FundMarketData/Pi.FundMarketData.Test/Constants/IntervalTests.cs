// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class IntervalTests
{
    public class GetIntervalDateTimes_Test
    {
        [Theory]
        [InlineData(Interval.Over3Months, -3, 0, 0)]
        [InlineData(Interval.Over6Months, -6, 0, 0)]
        [InlineData(Interval.Over1Year, 0, -1, 0)]
        [InlineData(Interval.Over3Years, 0, -3, 0)]
        [InlineData(Interval.Over5Years, 0, -5, 0)]
        [InlineData(Interval.YearToDate, 0, 0, 1)]
        [InlineData(Interval.SinceInception, 0, 0, 0, true)]
        public void WhenValidInterval_ReturnsExpectedDateTimes(Interval interval, int months, int years, int startMonth, bool isSinceInception = false)
        {
            // Arrange
            DateTime end = DateTime.UtcNow.Date;
            DateTime expectedStart = isSinceInception ? DateTime.MinValue : end.AddMonths(months).AddYears(years);
            if (startMonth > 0)
            {
                expectedStart = new DateTime(end.Year, startMonth, 1);
            }

            // Act
            var (start, endDate) = interval.GetIntervalDateTimes();

            // Assert
            start.Should().Be(expectedStart);
            endDate.Should().Be(end);
        }

        [Fact]
        public void WhenInvalidInterval_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidInterval = (Interval)999;

            // Act
            Action act = () => invalidInterval.GetIntervalDateTimes();

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Not expected Interval value: 999. (Parameter 'interval')");
        }
    }
}
