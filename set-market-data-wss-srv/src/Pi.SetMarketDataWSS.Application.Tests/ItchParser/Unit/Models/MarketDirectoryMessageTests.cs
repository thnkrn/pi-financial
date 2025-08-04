using System.Reflection;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public class MarketDirectoryMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.m
    );

    [Theory]
    [MemberData(nameof(message))]
    public void Parse_MarketDirectoryMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        MarketDirectoryMessage? output = itchParserService.Parse(input) as MarketDirectoryMessage;

        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], Int32.Parse(output.Nanos.ToString()));
        Assert.Equal(result[2], Int32.Parse(output.MarketCode.ToString()));
        Assert.Equal(result[3], output.MarketName.ToString());
        Assert.Equal(result[4], output.MarketDescription.ToString());
    }

    [Fact]
    public void Parse_MarketDirectoryMessage_VerifyToStringOutput()
    {
        // Arrange
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "MarketDirectoryMessage",
            "m.bin"
        );
        string expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "MarketDirectoryMessage",
            "m.txt"
        );

        // Ensure the files exist to avoid confusing errors during test runs
        Assert.True(File.Exists(binFilePath), $"Mock binary file not found: {binFilePath}");
        Assert.True(
            File.Exists(expectedOutputFilePath),
            $"Expected output file not found: {expectedOutputFilePath}"
        );

        // Read the expected output from m.txt
        string expectedOutput = File.ReadAllText(expectedOutputFilePath);

        // Read the binary data from m.bin
        byte[] marketDirectoryData = File.ReadAllBytes(binFilePath);

        // Act
        var marketDirectoryMessage =
            itchParserService.Parse(marketDirectoryData) as MarketDirectoryMessage;

        // Ensure parsing succeeded and resulted in the correct type
        Assert.NotNull(marketDirectoryMessage);

        // Split the expected output into parts for comparison
        var expectedParts = expectedOutput.Split('\n');

        // Assert - Compare each field
        Assert.Equal(uint.Parse(expectedParts[1]), marketDirectoryMessage.Nanos.Value);
        Assert.Equal(byte.Parse(expectedParts[2]), marketDirectoryMessage.MarketCode.Value);
        Assert.Equal(expectedParts[3], marketDirectoryMessage.MarketName);
        Assert.Equal(expectedParts[4], marketDirectoryMessage.MarketDescription);
    }
}
