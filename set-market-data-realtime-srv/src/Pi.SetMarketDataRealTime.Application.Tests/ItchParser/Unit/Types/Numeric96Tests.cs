using System.Numerics;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Types;

public class Numeric96Tests
{
    [Fact]
    public void Constructor_CorrectlyInitializesValue()
    {
        // Arrange
        byte[] bytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x02];
        var expected = new BigInteger(
            new byte[]
            {
                0x02,
                0x00,
                0x00,
                0x00,
                0x00,
                0x01,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            }
        );

        // Act
        var numeric = new Numeric96(bytes);

        // Assert
        Assert.Equal(expected, numeric);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_ForIncorrectLength()
    {
        // Arrange
        byte[] invalidBytes = [0x00, 0x00, 0x00]; // Less than 12 bytes

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Numeric96(invalidBytes));
    }

    [Fact]
    public void ImplicitConversion_ReturnsCorrectBigIntegerValue()
    {
        // Arrange
        byte[] bytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x02];
        var expected = new BigInteger(
            new byte[]
            {
                0x02,
                0x00,
                0x00,
                0x00,
                0x00,
                0x01,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00
            }
        );

        var numeric = new Numeric96(bytes);

        // Act
        BigInteger actual = numeric;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToString_ReturnsCorrectStringValue()
    {
        // Arrange
        byte[] bytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x02];
        var expected = BitConverter.ToString(bytes).Replace("-", string.Empty);
        var numeric = new Numeric96(bytes);

        // Act
        var actual = numeric.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}