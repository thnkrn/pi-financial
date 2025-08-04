using System.Reflection;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class MarketDirectoryMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.m
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(message))]
    public async Task Parse_MarketDirectoryMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as MarketDirectoryMessage;

        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], int.Parse(output.Nanos.ToString()));
        Assert.Equal(result[2], int.Parse(output.MarketCode.ToString()));
        Assert.Equal(result[3], output.MarketName.ToString());
        Assert.Equal(result[4], output.MarketDescription.ToString());
    }

    [Fact]
    public async Task Parse_MarketDirectoryMessage_VerifyToStringOutput()
    {
        // Arrange
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "MarketDirectoryMessage",
            "m.bin"
        );
        var expectedOutputFilePath = Path.Combine(
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
        var expectedOutput = File.ReadAllText(expectedOutputFilePath);

        // Read the binary data from m.bin
        var marketDirectoryData = File.ReadAllBytes(binFilePath);

        // Act
        var marketDirectoryMessage =
            await itchParserService.Parse(marketDirectoryData) as MarketDirectoryMessage;

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