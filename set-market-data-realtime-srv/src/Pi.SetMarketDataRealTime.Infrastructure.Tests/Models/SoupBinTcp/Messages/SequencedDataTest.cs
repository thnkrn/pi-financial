using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

public class SequencedDataTests
{
    private const char SequencedDataType = 'S';

    [Fact]
    public void Constructor_CreatesCorrectBytesArray_WithMessageTypeAndData()
    {
        var messageContent = new byte[] { 1, 2, 3, 4 };

        var sequencedData = new SequencedData(messageContent);

        Assert.Equal(SequencedDataType, (char)sequencedData.Bytes[0]);
        Assert.Equal(messageContent.Length + 1, sequencedData.Length);
        Assert.True(messageContent.SequenceEqual(sequencedData.Message));
    }

    [Fact]
    public void Message_ReturnsOriginalMessage_ExcludingTypeByte()
    {
        var messageContent = new byte[] { 1, 2, 3, 4 };
        var sequencedData = new SequencedData(messageContent);

        var actualMessage = sequencedData.Message;

        Assert.Equal(messageContent, actualMessage);
    }
}