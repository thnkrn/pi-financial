using System.Reflection;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public class PriceLimitMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.k
    );

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public void Parse_PriceLimitMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        PriceLimitMessage? output = itchParserService.Parse(input) as PriceLimitMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderbookId.Value);
        Assert.Equal(result[3], output.UpperLimit.Value);
        Assert.Equal(result[4], output.LowerLimit.Value);
    }

    [Fact]
    public void Parse_PriceLimitMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.k, 0, 0, 0, 1];

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }

    [Fact]
    public void Parse_PriceLimitMessage_VerifyToStringOutput()
    {
        // Arrange
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        string binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "PriceLimitMessage",
            "k.bin"
        );
        string expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "PriceLimitMessage",
            "k.txt"
        );

        // Read the expected output from k.txt
        string expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from k.bin
        byte[] priceLimitData = File.ReadAllBytes(binFilePath);

        // Act

        var priceLimitMessage = itchParserService.Parse(priceLimitData) as PriceLimitMessage;

        var expectedParts = expectedOutput.Split('\n');

        // Assert
        Assert.Equal(int.Parse(expectedParts[1]), (int)priceLimitMessage!.Nanos.Value); // Nanos
        Assert.Equal(int.Parse(expectedParts[2]), (int)priceLimitMessage.OrderbookId.Value); // OrderbookId
        Assert.Equal(int.Parse(expectedParts[3]), priceLimitMessage.UpperLimit.Value); // UpperLimit
        Assert.Equal(int.Parse(expectedParts[4]), priceLimitMessage.LowerLimit.Value); // LowerLimit
    }
}
