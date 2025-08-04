using System.Text;
using Pi.SetMarketData.Application.Services.Types.ItchParser;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Types;

public class AlphaTests
{
    [Fact]
    public void Constructor_SingleCharacterAlphaData_CorrectPadding()
    {
        // Arrange
        byte[] inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("A");
        int fieldLength = 5;

        // Act
        Alpha alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("A    ", alpha.Value);
    }

    [Fact]
    public void Constructor_CharacterAlphaDataExactMatch()
    {
        // Arrange
        byte[] inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("ABCDE");
        int fieldLength = 5;

        // Act
        Alpha alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("ABCDE", alpha.Value);
    }

    [Fact]
    public void Constructor_AlphaNumericStringWithISO8859_1Characters_CorrectRepresentation()
    {
        // Arrange
        byte[] inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("1234ÄÖ");
        int fieldLength = 10;

        // Act
        Alpha alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("1234ÄÖ    ", alpha.Value);
    }

    [Fact]
    public void Constructor_LongAlphaStringWithPadding_TruncatedToFieldLength()
    {
        // Arrange
        byte[] inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("LONGALPHA STRING123");
        int fieldLength = 10;

        // Act
        Alpha alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("LONGALPHA ", alpha.Value);
    }
}
