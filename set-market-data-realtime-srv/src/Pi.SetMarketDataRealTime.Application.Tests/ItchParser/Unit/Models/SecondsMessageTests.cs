using System.Reflection;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class SecondsMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.T
    );

    private readonly ITestOutputHelper _testOutputHelper;

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    public SecondsMessageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    private async Task Parse_CorrectlyFormattedSecondsMessage_ReturnsCorrectValues()
    {
        // Arrange
        byte[] bytes = [(byte)ItchMessageType.T, 0, 0, 0, 1]; // Message type 'T' with Unix time 1
        // Act
        var message = await itchParserService.Parse(bytes) as SecondsMessage;

        // Assert
        Assert.NotNull(message);
        Assert.Equal(ItchMessageType.T, message.MsgType);
        Assert.Equal((uint)1, message.Second);
    }

    [Fact]
    public async Task Parse_IncorrectlyFormattedMessage_ThrowsException()
    {
        // Arrange
        byte[] bytes = [(byte)'X']; // Unsupported message type 'X'

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () => await itchParserService.Parse(bytes));
    }

    [Fact]
    public async Task Parse_InsufficientData_ThrowsException()
    {
        // Arrange
        byte[] bytes = [(byte)ItchMessageType.T, 0, 0, 0]; // Missing one byte for a valid SecondsMessage

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(bytes));
        Assert.True(
            exception is ArgumentException,
            "Expected ArgumentException for insufficient data."
        );
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(uint.MinValue)]
    public async Task Parse_EdgeCasesUnixTimeValues_ParsesCorrectly(uint unixTime)
    {
        // Arrange
        var timeBytes = BitConverter.GetBytes(unixTime);
        if (BitConverter.IsLittleEndian) Array.Reverse(timeBytes);

        var bytes = new byte[5];
        bytes[0] = (byte)ItchMessageType.T;
        Array.Copy(timeBytes, 0, bytes, 1, 4);
        // Act
        var message = await itchParserService.Parse(bytes) as SecondsMessage;

        // Assert
        Assert.NotNull(message);
        Assert.Equal(unixTime, message.Second);
    }

    [Fact]
    public async Task Parse_SecondsMessage_VerifyToStringOutput()
    {
        // Arrange
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "SecondsMessage",
            "T.bin"
        );
        var expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "SecondsMessage",
            "T.txt"
        );

        // Read the expected output from k.txt
        var expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from k.bin
        var secondsData = File.ReadAllBytes(binFilePath);

        // Act
        var secondsMessage = await itchParserService.Parse(secondsData) as SecondsMessage;
        var actualOutput = secondsMessage!.ToString().Trim();

        var expectedParts = expectedOutput.Split('\n');
        var actualParts = actualOutput.Split(
            new[] { '\n', ',', ':' },
            StringSplitOptions.RemoveEmptyEntries
        );

        _testOutputHelper.WriteLine(actualParts.Length.ToString());
        // Trim all parts to avoid failing due to leading/trailing whitespace
        for (var i = 0; i < actualParts.Length; i++)
            actualParts[i] = actualParts[i].Trim();

        // Assert
        Assert.Equal(expectedParts[1], actualParts[1]);
    }

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public async Task Parse_SecondsMessage_VerifyWithMockMessageData(byte[] input, object[] result)
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as SecondsMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Second.Value);
    }
}