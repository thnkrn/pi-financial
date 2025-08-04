using System.Reflection;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class TickSizeMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.L
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public async Task Parse_TickSizeMessage_VerifyBehavior(byte[] input, object[] result)
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as TickSizeMessage;

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
    public async Task Parse_TickSizeMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.L, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }

    [Fact]
    public async Task Parse_TickSizeMessage_VerifyToStringOutput()
    {
        // Arrange
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "TickSizeMessage",
            "L.bin"
        );
        var expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "TickSizeMessage",
            "L.txt"
        );

        // Read the expected output from example.txt
        var expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from example.bin
        var tickSizeData = File.ReadAllBytes(binFilePath);

        // Act
        var tickSizeMessage = await itchParserService.Parse(tickSizeData) as TickSizeMessage;
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