using System.Reflection;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;

public class TickSizeMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.L
    );

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public void Parse_TickSizeMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        TickSizeMessage? output = itchParserService.Parse(input) as TickSizeMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderBookId.Value);
        Assert.Equal(Convert.ToInt64(result[3]), output.TickSize.Value);
        Assert.Equal(result[4], output.PriceFrom.Value);
        Assert.Equal(result[5], output.PriceTo.Value);
    }

    [Fact]
    public void Parse_TickSizeMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.L, 0, 0, 0, 1];

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }

    [Fact]
    public void Parse_TickSizeMessage_VerifyToStringOutput()
    {
        // Arrange
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        string binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "TickSizeMessage",
            "L.bin"
        );
        string expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "TickSizeMessage",
            "L.txt"
        );

        // Read the expected output from example.txt
        string expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from example.bin
        byte[] tickSizeData = File.ReadAllBytes(binFilePath);

        // Act
        var tickSizeMessage = itchParserService.Parse(tickSizeData) as TickSizeMessage;
        Assert.NotNull(tickSizeMessage); // Ensure parsing succeeded and resulted in the correct type

        // Extracting expected parts
        var expectedParts = expectedOutput.Split('\n');

        // Assert - Compare each field
        Assert.Equal(uint.Parse(expectedParts[1]), tickSizeMessage.Nanos);
        Assert.Equal(uint.Parse(expectedParts[2]), tickSizeMessage.OrderBookId);
        Assert.Equal(int.Parse(expectedParts[3]), tickSizeMessage.TickSize.Value);
        Assert.Equal(int.Parse(expectedParts[4]), tickSizeMessage.PriceFrom.Value);
        Assert.Equal(int.Parse(expectedParts[5]), tickSizeMessage.PriceTo.Value);
    }
}
