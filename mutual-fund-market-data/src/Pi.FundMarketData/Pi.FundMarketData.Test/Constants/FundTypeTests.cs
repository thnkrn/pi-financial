// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class FundTypeTests
{
    public class ParseFundType_Test
    {
        [Theory]
        [InlineData("P", FundType.Plain)]
        [InlineData("C", FundType.Complex)]
        [InlineData("X", FundType.Unknown)]
        public void WhenValidValue_ReturnsExpectedFundType(string value, FundType expected)
        {
            // Act
            var result = FundTypeExtension.ParseFundType(value);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void WhenInvalidValue_ReturnsUnknown()
        {
            // Arrange
            var invalidValue = "X";

            // Act
            var result = FundTypeExtension.ParseFundType(invalidValue);

            // Assert
            result.Should().Be(FundType.Unknown);
        }
    }
}
