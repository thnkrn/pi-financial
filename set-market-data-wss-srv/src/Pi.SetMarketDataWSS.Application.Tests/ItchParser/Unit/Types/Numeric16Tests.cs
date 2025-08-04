using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class Numeric16Tests
{
    [Fact]
    public void Constructor_CorrectlyParsesBytes()
    {
        // Arrange
        byte[] bigEndianBytes = { 0x01, 0x02 }; // Equivalent to 258 in big-endian
        ushort expected = 258;

        // Act
        Numeric16 numeric = new Numeric16(bigEndianBytes);

        // Assert
        Assert.Equal(expected, numeric.Value);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForIncorrectLength()
    {
        // Arrange
        byte[] invalidBytes = { 0x00 }; // Less than 2 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric16(invalidBytes));
    }

    [Fact]
    public void ImplicitConversion_ReturnsCorrectUShortValue()
    {
        // Arrange
        byte[] bigEndianBytes = { 0x01, 0x02 };
        ushort expected = 258;
        Numeric16 numeric = new Numeric16(bigEndianBytes);

        // Act
        ushort actual = numeric;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValue()
    {
        // Arrange
        byte[] bigEndianBytes = { 0x01, 0x02 };
        ushort value = 258;
        string expected = value.ToString();
        Numeric16 numeric = new Numeric16(bigEndianBytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}
