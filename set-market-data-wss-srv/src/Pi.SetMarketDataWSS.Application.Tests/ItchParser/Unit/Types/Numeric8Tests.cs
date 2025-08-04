using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class Numeric8Tests
{
    [Fact]
    public void Constructor_CorrectlyInitializesValue()
    {
        // Arrange 
        byte expected = 0x01;
        byte[] bytes = { expected };

        // Act
        Numeric8 numeric = new Numeric8(bytes);

        // Assert
        Assert.Equal(expected, numeric.Value);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForNullArray()
    {
        // Arrange
        byte[] bytes = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric8(bytes));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForIncorrectLength()
    {
        // Arrange
        byte[] invalidBytes = { 0x00, 0x02 }; // Array with more than one byte

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric8(invalidBytes));
    }

    [Fact]
    public void ImplicitConversion_ReturnsCorrectByteValue()
    {
        // Arrange
        byte expected = 0x01;
        byte[] bytes = { expected };
        Numeric8 numeric = new Numeric8(bytes);

        // Act
        byte actual = numeric;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValue()
    {
        // Arrange
        byte value = 0x01;
        byte[] bytes = { value };
        string expected = value.ToString();
        Numeric8 numeric = new Numeric8(bytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValueForMinValue()
    {
        // Arrange
        byte value = 0x00;
        byte[] bytes = { value };
        string expected = "0";
        Numeric8 numeric = new Numeric8(bytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValueForMaxValue()
    {
        // Arrange
        byte value = 0xFF;
        byte[] bytes = { value };
        string expected = "255";
        Numeric8 numeric = new Numeric8(bytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}
