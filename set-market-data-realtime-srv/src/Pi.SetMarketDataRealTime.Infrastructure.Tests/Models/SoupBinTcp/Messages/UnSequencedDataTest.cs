using System.Text;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

public class UnSequencedDataTests
{
    private const char UnSequencedDataType = 'U';

    [Fact]
    public void Constructor_PrependTypeIdentifier_ToMessageBytes()
    {
        var originalMessage = Encoding.ASCII.GetBytes("SampleMessage");
        var expectedBytes = new byte[originalMessage.Length + 1];
        expectedBytes[0] = Convert.ToByte(UnSequencedDataType);
        Array.Copy(originalMessage, 0, expectedBytes, 1, originalMessage.Length);

        var unSequencedData = new UnSequencedData(originalMessage);

        Assert.Equal(expectedBytes, unSequencedData.Bytes);
    }

    [Fact]
    public void MessageProperty_ExcludesTypeIdentifier_ReturnsOriginalMessage()
    {
        var originalMessage = Encoding.ASCII.GetBytes("SampleMessage");
        var unSequencedData = new UnSequencedData(originalMessage);

        var messageBytes = unSequencedData.Message;

        Assert.Equal(originalMessage, messageBytes);
    }
}