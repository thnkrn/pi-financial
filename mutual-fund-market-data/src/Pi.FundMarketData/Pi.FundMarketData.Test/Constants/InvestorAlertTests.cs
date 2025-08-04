// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class InvestorAlertTests
{
    public class ParseInvestorAlerts_Test
    {
        [Theory]
        [InlineData(new[] { "1", "2" }, new[] { InvestorAlert.RiskFromConcentratedInvestment, InvestorAlert.ConsolidationFund })]
        [InlineData(new[] { "0", "3" }, new[] { InvestorAlert.Unknown, InvestorAlert.Unknown })]
        [InlineData(new[] { " ", "1" }, new[] { InvestorAlert.RiskFromConcentratedInvestment })]
        [InlineData(new string[] { }, new InvestorAlert[] { })]
        public void WhenValidValues_ReturnsExpectedInvestorAlerts(IEnumerable<string> values, IEnumerable<InvestorAlert> expected)
        {
            // Act
            var result = InvestorAlertExtension.ParseInvestorAlerts(values);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WhenInvalidValues_ReturnsUnknown()
        {
            // Arrange
            var values = new[] { "invalid", "4" };

            // Act
            var result = InvestorAlertExtension.ParseInvestorAlerts(values);

            // Assert
            result.Should().AllBeEquivalentTo(InvestorAlert.Unknown);
        }
    }
}
