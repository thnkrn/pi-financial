using System.Text;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Types;

public class AlphaTests
{
    [Fact]
    public void Constructor_SingleCharacterAlphaData_CorrectPadding()
    {
        // Arrange
        var inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("A");
        var fieldLength = 5;

        // Act
        var alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("A    ", alpha.Value);
    }

    [Fact]
    public void Constructor_CharacterAlphaDataExactMatch()
    {
        // Arrange
        var inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("ABCDE");
        var fieldLength = 5;

        // Act
        var alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("ABCDE", alpha.Value);
    }

    [Fact]
    public void Constructor_AlphaNumericStringWithISO8859_1Characters_CorrectRepresentation()
    {
        // Arrange
        var inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("1234ÄÖ");
        var fieldLength = 10;

        // Act
        var alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("1234ÄÖ    ", alpha.Value);
    }

    [Fact]
    public void Constructor_LongAlphaStringWithPadding_TruncatedToFieldLength()
    {
        // Arrange
        var inputData = Encoding.GetEncoding("ISO-8859-1").GetBytes("LONGALPHA STRING123");
        var fieldLength = 10;

        // Act
        var alpha = new Alpha(inputData, fieldLength);

        // Assert
        Assert.Equal("LONGALPHA ", alpha.Value);
    }
}