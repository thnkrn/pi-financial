using System.Reflection;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class PriceLimitMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.k
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public async Task Parse_PriceLimitMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as PriceLimitMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderbookId.Value);
        Assert.Equal(result[3], output.UpperLimit.Value);
        Assert.Equal(result[4], output.LowerLimit.Value);
    }

    [Fact]
    public async Task Parse_PriceLimitMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.k, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }

    [Fact]
    public async Task Parse_PriceLimitMessage_VerifyToStringOutput()
    {
        // Arrange
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "PriceLimitMessage",
            "k.bin"
        );
        var expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "PriceLimitMessage",
            "k.txt"
        );

        // Read the expected output from k.txt
        var expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from k.bin
        var priceLimitData = File.ReadAllBytes(binFilePath);

        // Act

        var priceLimitMessage = await itchParserService.Parse(priceLimitData) as PriceLimitMessage;

        var expectedParts = expectedOutput.Split('\n');

        // Assert
        Assert.Equal(int.Parse(expectedParts[1]), (int)priceLimitMessage!.Nanos.Value); // Nanos
        Assert.Equal(int.Parse(expectedParts[2]), (int)priceLimitMessage.OrderbookId.Value); // OrderbookId
        Assert.Equal(int.Parse(expectedParts[3]), priceLimitMessage.UpperLimit.Value); // UpperLimit
        Assert.Equal(int.Parse(expectedParts[4]), priceLimitMessage.LowerLimit.Value); // LowerLimit
    }
}