// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Moq;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class PurchaseTests
{
    private class TestablePurchase : Purchase
    {
        private readonly DateTime _fixedMarketTime;

        public TestablePurchase(DateTime fixedMarketTime)
        {
            _fixedMarketTime = fixedMarketTime;
        }

        protected override DateTime GetMarketTimeLocalAsUtc()
        {
            return _fixedMarketTime;
        }
    }
    public class GetBuyCutOffTimeLocal_Test
    {
        [Fact]
        public void WhenBuyCutOffTimeIsValid_ReturnsExpectedDateTime()
        {
            // Arrange
            var purchase = new Purchase { BuyCutOffTime = 1530 };

            // Act
            var result = purchase.GetBuyCutOffTimeLocal();

            // Assert
            result.Should().HaveHour(15).And.HaveMinute(30);
        }

        [Fact]
        public void WhenBuyCutOffTimeIsNull_ReturnsNull()
        {
            // Arrange
            var purchase = new Purchase { BuyCutOffTime = null };

            // Act
            var result = purchase.GetBuyCutOffTimeLocal();

            // Assert
            result.Should().BeNull();
        }
    }

    public class GetSellCutOffTimeLocal_Test
    {
        [Fact]
        public void WhenSellCutOffTimeIsValid_ReturnsExpectedDateTime()
        {
            // Arrange
            var purchase = new Purchase { SellCutOffTime = 1530 };

            // Act
            var result = purchase.GetSellCutOffTimeLocal();

            // Assert
            result.Should().HaveHour(15).And.HaveMinute(30);
        }

        [Fact]
        public void WhenSellCutOffTimeIsNull_ReturnsNull()
        {
            // Arrange
            var purchase = new Purchase { SellCutOffTime = null };

            // Act
            var result = purchase.GetSellCutOffTimeLocal();

            // Assert
            result.Should().BeNull();
        }
    }

    public class GetPiBuyCutOffTimeLocal_Test
    {
        [Fact]
        public void WhenBuyCutOffTimeIsValid_ReturnsExpectedDateTime()
        {
            // Arrange
            var purchase = new Purchase { BuyCutOffTime = 1530 };

            // Act
            var result = purchase.GetPiBuyCutOffTimeLocal();

            // Assert
            result.Should().Be(purchase.GetBuyCutOffTimeLocal() - StaticConfig.CutOffTimeDeduction);
        }

        [Fact]
        public void WhenBuyCutOffTimeIsNull_ReturnsDefaultCutOffTime()
        {
            // Arrange
            var purchase = new Purchase { BuyCutOffTime = null };

            // Act
            var result = purchase.GetPiBuyCutOffTimeLocal();

            // Assert
            result.Should().Be(purchase.GetPiBuyCutOffTimeLocal() - StaticConfig.CutOffTimeDeduction);
        }
    }

    public class GetPiSellCutOffTimeLocal_Test
    {
        [Fact]
        public void WhenSellCutOffTimeIsValid_ReturnsExpectedDateTime()
        {
            // Arrange
            var purchase = new Purchase { SellCutOffTime = 1530 };

            // Act
            var result = purchase.GetPiSellCutOffTimeLocal();

            // Assert
            result.Should().Be(purchase.GetSellCutOffTimeLocal() - StaticConfig.CutOffTimeDeduction);
        }

        [Fact]
        public void WhenSellCutOffTimeIsNull_ReturnsDefaultCutOffTime()
        {
            // Arrange
            var purchase = new Purchase { SellCutOffTime = null };

            // Act
            var result = purchase.GetPiSellCutOffTimeLocal();

            // Assert
            result.Should().Be(purchase.GetPiSellCutOffTimeLocal() - StaticConfig.CutOffTimeDeduction);
        }
    }

    public class GetClosestTradeDate_Test
    {
        public static TheoryData<DateTime, int, TradeSide, DateTime> BeforeCutOffTestData =>
            new TheoryData<DateTime, int, TradeSide, DateTime>
            {
                    { new DateTime(2024, 12, 25, 9, 0, 0), 1530, TradeSide.Buy, new DateTime(2024, 12, 25) },
                // { new DateTime(2024, 12, 25), 1430, TradeSide.Buy, new DateTime(2024, 12, 25) },
                // { new DateTime(2024, 12, 25), 1200, TradeSide.Buy, new DateTime(2024, 12, 25) }
            };

        public static TheoryData<DateTime, int, TradeSide, DateTime> AfterCutOffTestData =>
            new TheoryData<DateTime, int, TradeSide, DateTime>
            {
                    { new DateTime(2024, 12, 25, 17, 0, 0), 1530, TradeSide.Buy, new DateTime(2024, 12, 26) },
                    { new DateTime(2024, 12, 25, 17, 0, 0), 1430, TradeSide.Buy, new DateTime(2024, 12, 26) },
                    { new DateTime(2024, 12, 25, 17, 0, 0), 1200, TradeSide.Buy, new DateTime(2024, 12, 26) }
            };

        [Theory]
        [MemberData(nameof(BeforeCutOffTestData))]
        public void WhenMarketTimeIsBeforeCutOff_ReturnsSameDay(DateTime marketTime, int cutOffTime, TradeSide tradeSide, DateTime expectedDate)
        {
            // Arrange
            var purchase = new TestablePurchase(marketTime)
            {
                BuyCutOffTime = tradeSide == TradeSide.Buy ? cutOffTime : (int?)null,
                SellCutOffTime = tradeSide == TradeSide.Sell ? cutOffTime : (int?)null
            };

            // Act
            var result = purchase.GetClosestTradeDate(tradeSide);

            // Assert
            result.Should().Be(expectedDate);
        }

        [Theory]
        [MemberData(nameof(AfterCutOffTestData))]
        public void WhenMarketTimeIsAfterCutOff_ReturnsNextDay(DateTime marketTime, int cutOffTime, TradeSide tradeSide, DateTime expectedDate)
        {
            // Arrange
            var purchase = new TestablePurchase(marketTime)
            {
                BuyCutOffTime = tradeSide == TradeSide.Buy ? cutOffTime : (int?)null,
                SellCutOffTime = tradeSide == TradeSide.Sell ? cutOffTime : (int?)null
            };

            // Act
            var result = purchase.GetClosestTradeDate(tradeSide);

            // Assert
            result.Should().Be(expectedDate);
        }

        [Fact]
        public void WhenInvalidTradeSide_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var purchase = new Purchase();
            var invalidTradeSide = (TradeSide)999;

            // Act
            Action act = () => purchase.GetClosestTradeDate(invalidTradeSide);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Not expected TradeSide value: 999. (Parameter 'side')");
        }
    }
}
