using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Types;

public class Numeric8Tests
{
    [Fact]
    public void Constructor_CorrectlyInitializesValue()
    {
        // Arrange 
        byte expected = 0x01;
        byte[] bytes = { expected };

        // Act
        var numeric = new Numeric8(bytes);

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
        var numeric = new Numeric8(bytes);

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
        var expected = value.ToString();
        var numeric = new Numeric8(bytes);

        // Act
        var actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValueForMinValue()
    {
        // Arrange
        byte value = 0x00;
        byte[] bytes = { value };
        var expected = "0";
        var numeric = new Numeric8(bytes);

        // Act
        var actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValueForMaxValue()
    {
        // Arrange
        byte value = 0xFF;
        byte[] bytes = { value };
        var expected = "255";
        var numeric = new Numeric8(bytes);

        // Act
        var actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}