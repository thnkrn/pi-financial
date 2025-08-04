using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Tests.Utils;

public class DataManipulationTest
{
    [Fact]
    public void FormatDecimal_Case_No_Decimals_On_Value()
    {
        // Arrange
        var value = "12350";
        var decimals = 2;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("123.50", result);
    }

    [Fact]
    public void FormatDecimal_Case_Decimals_Greater_Than_Value_Length()
    {
        // Arrange
        var value = "123";
        var decimals = 5;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("0.00123", result);
    }

    [Fact]
    public void FormatDecimal_Case_Value_With_Existing_Decimal_Point()
    {
        // Arrange
        var value = "123.45";
        var decimals = 3;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("12.345", result);
    }

    [Fact]
    public void FormatDecimal_Case_Empty_Value()
    {
        // Arrange
        var value = "";
        var decimals = 2;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("0.00", result);
    }

    [Fact]
    public void FormatDecimal_Case_Decimals_Zero_With_Decimal_Point()
    {
        // Arrange
        var value = "123.456";
        var decimals = 0;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("123.456", result);
    }

    [Fact]
    public void FormatDecimal_Case_Decimals_Zero_Without_Decimal_Point()
    {
        // Arrange
        var value = "12345";
        var decimals = 0;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals);

        // Assert
        Assert.Equal("12345.00", result);
    }

    [Fact]
    public void FormatDecimal_Case_Handle_Unavailable_Value()
    {
        // Arrange
        var value = "-2147483648";
        var decimals = 2;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals, true);

        // Assert
        Assert.Equal("0.00", result);
    }


    [Fact]
    public void FormatDecimal_Case_Fixed_Decimal_Had_Value()
    {
        // Arrange
        var value = "2555556";
        var decimals = 5;
        var fixedDecimal = 2;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals, false, fixedDecimal);

        // Assert
        Assert.Equal("25.56", result);
    }

    [Fact]
    public void FormatDecimal_Case_Fixed_Decimal_Had_Zero_As_Value()
    {
        // Arrange
        var value = "0";
        var decimals = 5;
        var fixedDecimal = 2;

        // Act
        var result = DataManipulation.FormatDecimals(value, decimals, false, fixedDecimal);

        // Assert
        Assert.Equal("0.00", result);
    }
    [Fact]
    public void FormatDecimal_ConvertNegativeValue()
    {
        var value = "-3000";
        var decimals = 4;

        // Act 
        var result = DataManipulation.FormatDecimals(value, decimals, false, 0);

        // Assert 
        Assert.Equal("-0.30", result);
    }
}