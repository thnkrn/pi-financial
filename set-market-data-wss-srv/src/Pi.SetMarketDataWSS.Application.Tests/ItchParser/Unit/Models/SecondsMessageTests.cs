using System.Reflection;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public class SecondsMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.T
    );

    [Fact]
    private void Parse_CorrectlyFormattedSecondsMessage_ReturnsCorrectValues()
    {
        // Arrange
        byte[] bytes = [(byte)ItchMessageType.T, 0, 0, 0, 1]; // Message type 'T' with Unix time 1
        // Act
        var message = itchParserService.Parse(bytes) as SecondsMessage;

        // Assert
        Assert.NotNull(message);
        Assert.Equal(ItchMessageType.T, message.MsgType);
        Assert.Equal((uint)1, message.Second);
    }

    [Fact]
    public void Parse_IncorrectlyFormattedMessage_ThrowsException()
    {
        // Arrange
        byte[] bytes = [(byte)'X']; // Unsupported message type 'X'

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => itchParserService.Parse(bytes));
    }

    [Fact]
    public void Parse_InsufficientData_ThrowsException()
    {
        // Arrange
        byte[] bytes = [(byte)ItchMessageType.T, 0, 0, 0]; // Missing one byte for a valid SecondsMessage

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => itchParserService.Parse(bytes));
        Assert.True(
            exception is ArgumentException,
            "Expected ArgumentException for insufficient data."
        );
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(uint.MinValue)]
    public void Parse_EdgeCasesUnixTimeValues_ParsesCorrectly(uint unixTime)
    {
        // Arrange
        byte[] timeBytes = BitConverter.GetBytes(unixTime);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timeBytes);
        }
        byte[] bytes = new byte[5];
        bytes[0] = (byte)ItchMessageType.T;
        Array.Copy(timeBytes, 0, bytes, 1, 4);
        // Act
        var message = itchParserService.Parse(bytes) as SecondsMessage;

        // Assert
        Assert.NotNull(message);
        Assert.Equal(unixTime, message.Second);
    }

    [Fact]
    public void Parse_SecondsMessage_VerifyToStringOutput()
    {
        // Arrange
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        string binFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "SecondsMessage",
            "T.bin"
        );
        string expectedOutputFilePath = Path.Combine(
            basePath,
            "ItchParser",
            "Mocks",
            "SecondsMessage",
            "T.txt"
        );

        // Read the expected output from k.txt
        string expectedOutput = File.ReadAllText(expectedOutputFilePath).Trim();

        // Read the binary data from k.bin
        byte[] secondsData = File.ReadAllBytes(binFilePath);

        // Act
        var secondsMessage = itchParserService.Parse(secondsData) as SecondsMessage;
        string actualOutput = secondsMessage!.ToString().Trim();

        var expectedParts = expectedOutput.Split('\n');
        var actualParts = actualOutput.Split(
            new[] { '\n', ',', ':' },
            StringSplitOptions.RemoveEmptyEntries
        );

        // Trim all parts to avoid failing due to leading/trailing whitespace
        for (int i = 0; i < actualParts.Length; i++)
            actualParts[i] = actualParts[i].Trim();

        // Assert
        Assert.Equal(expectedParts[1], actualParts[2]);
    }

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public void Parse_SecondsMessage_VerifyWithMockMessageData(byte[] input, object[] result)
    {
        // Arrange
        // Act
        SecondsMessage? output = itchParserService.Parse(input) as SecondsMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Second.Value);
    }
}
