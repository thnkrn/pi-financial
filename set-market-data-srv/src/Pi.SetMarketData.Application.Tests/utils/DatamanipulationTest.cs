using Pi.SetMarketData.Application.Utils;
using Xunit;

namespace Pi.SetMarketData.Application.Tests.Utils
{
    public class DataManipulationTest
    {
        [Fact]
        public void TestFormatDecimal_Case_No_Decimals_On_Value()
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
        public void TestFormatDecimal_Case_Decimals_Greater_Than_Value_Length()
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
        public void TestFormatDecimal_Case_Value_With_Existing_Decimal_Point()
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
        public void TestFormatDecimal_Case_Empty_Value()
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
        public void TestFormatDecimal_Case_Decimals_Zero_With_Decimal_Point()
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
        public void TestFormatDecimal_Case_Decimals_Zero_Without_Decimal_Point()
        {
            // Arrange
            var value = "12345";
            var decimals = 0;

            // Act
            var result = DataManipulation.FormatDecimals(value, decimals);

            // Assert
            Assert.Equal("12345.00", result);
        }

        [Theory]
        [InlineData("", "")]
        public void CalculateDaysUntilDate_ShouldReturnEmptyString(string value, string expect)
        {
            // Act
            var result = DataManipulation.CalculateDaysUntilDate(value);

            // Assert
            Assert.Equal(expect, result);
        }
    }
}
