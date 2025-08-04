using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class Numeric32Tests
{
    [Fact]
    public void Constructor_CorrectlyParsesBytes()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x01, 0x02]; // Equivalent to 258 in big-endian
        uint expected = 258;

        // Act
        Numeric32 numeric = new Numeric32(bigEndianBytes);

        // Assert
        Assert.Equal(expected, numeric.Value);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForIncorrectLength()
    {
        // Arrange
        byte[] invalidBytes = [0x00, 0x00]; // Less than 4 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric32(invalidBytes));
    }

    [Fact]
    public void ImplicitConversion_ReturnsCorrectUintValue()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x01, 0x02]; // Equivalent to 258 in big-endian
        uint expected = 258;
        Numeric32 numeric = new Numeric32(bigEndianBytes);

        // Act
        uint actual = numeric;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValue()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x01, 0x02]; // Equivalent to 258 in big-endian
        string expected = "258";
        Numeric32 numeric = new Numeric32(bigEndianBytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}
