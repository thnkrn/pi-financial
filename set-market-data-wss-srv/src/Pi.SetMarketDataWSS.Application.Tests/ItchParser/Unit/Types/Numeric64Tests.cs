using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class Numeric64Tests
{
    [Fact]
    public void Constructor_CorrectlyParsesBytes()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02]; // Example 64-bit value
        ulong expected = BitConverter.ToUInt64([0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00], 0); // Adjust for endianness

        // Act
        Numeric64 numeric = new Numeric64(bigEndianBytes);

        // Assert
        Assert.Equal(expected, numeric.Value);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForIncorrectLength()
    {
        // Arrange
        byte[] invalidBytes = [0x00, 0x00]; // Less than 8 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric64(invalidBytes));
    }

    [Fact]
    public void ImplicitConversion_ReturnsCorrectULongValue()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02];
        ulong expected = BitConverter.ToUInt64([0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00], 0);
        Numeric64 numeric = new Numeric64(bigEndianBytes);

        // Act
        ulong actual = numeric;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValue()
    {
        // Arrange
        byte[] bigEndianBytes = [0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02];
        ulong value = BitConverter.ToUInt64([0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00], 0);
        string expected = value.ToString();
        Numeric64 numeric = new Numeric64(bigEndianBytes);

        // Act
        string actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}
