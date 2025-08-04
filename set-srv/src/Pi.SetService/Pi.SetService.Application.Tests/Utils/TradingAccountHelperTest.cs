using Pi.SetService.Application.Utils;

namespace Pi.SetService.Application.Tests.Utils;

public class TradingAccountHelperTest
{
    [Theory]
    [InlineData("0900082-6", "0900082")]
    [InlineData("0900082-1", "0900082")]
    [InlineData("0900082-8", "0900082")]
    [InlineData("0900082-7", null)]
    [InlineData("09000827", null)]
    [InlineData("0900082", null)]
    public void Should_ReturnExpected_When_GetCustCodeBySetTradingAccountNo(string tradingAccountNo, string? expected)
    {
        // Act
        var actual = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(tradingAccountNo);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_ReturnExpected_When_GetCustCodeBySetAccountNo()
    {
        // Arrange
        var accountNo = "09000826";

        // Act
        var actual = TradingAccountHelper.GetCustCodeBySetAccountNo(accountNo);

        // Assert
        Assert.Equal("0900082", actual);
    }
}
