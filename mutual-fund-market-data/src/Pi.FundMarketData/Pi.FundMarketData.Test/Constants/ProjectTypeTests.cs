// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class ProjectTypeTests
{
    public class ParseProjectType_Test
    {
        [Theory]
        [InlineData("R", ProjectType.GeneralInvestorsFund)]
        [InlineData("N", ProjectType.InstitutionalInvestorsFund)]
        [InlineData("A", ProjectType.NonRetailInvestorsFund)]
        [InlineData("X", ProjectType.NonRetailAndHighNetWorthInvestorsFund)]
        [InlineData("H", ProjectType.InstitutionalAndSpecialLargeInvestorsFund)]
        [InlineData("B", ProjectType.HighNetWorthInvestorsFund)]
        [InlineData("J", ProjectType.CorporateInvestorsFund)]
        [InlineData("G", ProjectType.ThaiGovernmentFund)]
        [InlineData("V", ProjectType.PensionReserveFund)]
        [InlineData(" ", ProjectType.Unknown)]
        [InlineData("Z", ProjectType.Unknown)]
        public void WhenValidValue_ReturnsExpectedProjectType(string value, ProjectType expected)
        {
            // Act
            var result = ProjectTypeExtension.ParseProjectType(value);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void WhenInvalidValue_ReturnsUnknown()
        {
            // Arrange
            var invalidValue = "Z";

            // Act
            var result = ProjectTypeExtension.ParseProjectType(invalidValue);

            // Assert
            result.Should().Be(ProjectType.Unknown);
        }
    }
}
