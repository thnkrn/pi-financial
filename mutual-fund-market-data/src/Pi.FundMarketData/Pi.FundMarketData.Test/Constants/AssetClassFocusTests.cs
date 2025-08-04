// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Test.Constants;

public class AssetClassFocusTests
{
    public class GetAssetClassFocus_Test
    {
        [Theory]
        [InlineData("F", "Fixed Income")]
        [InlineData("E", "Equity")]
        [InlineData("M", "Mixed")]
        [InlineData("O", "Other")]
        public void WhenValidFirstLetter_ReturnsExpectedAssetClass(string firstLetter, string expected)
        {
            // Act
            var result = AssetClassFocus.GetAssetClassFocus(firstLetter);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void WhenInvalidFirstLetter_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidFirstLetter = "X";

            // Act
            Action act = () => AssetClassFocus.GetAssetClassFocus(invalidFirstLetter);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Not expected AssetClassFocus value: X. (Parameter 'firstLetter')");
        }
    }
}
