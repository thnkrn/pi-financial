using System.Text;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

public class ServerHeartbeatTests
{
    private const char ServerHeartbeatType = 'H';

    [Fact]
    public void Constructor_SetsBytesProperty_ToSingleCharH()
    {
        var heartbeat = new ServerHeartbeat();

        var expected = Encoding.ASCII.GetBytes(new[] { ServerHeartbeatType });
        Assert.Equal(expected, heartbeat.Bytes);
    }

    [Fact]
    public void Length_ReturnsCorrectLength_OfBytes()
    {
        var heartbeat = new ServerHeartbeat();

        Assert.Equal(1, heartbeat.Length);
    }

    [Fact]
    public void Type_ReturnsCorrectType_OfMessage()
    {
        var heartbeat = new ServerHeartbeat();

        Assert.Equal(ServerHeartbeatType, heartbeat.Type);
    }

    [Fact]
    public void TotalBytes_IncludesLengthPrefix_AndMessageBytes_ConsideringEndianness()
    {
        var heartbeat = new ServerHeartbeat();

        var totalBytes = heartbeat.TotalBytes;
        var expectedLength = BitConverter.GetBytes((ushort)1);
        if (BitConverter.IsLittleEndian) Array.Reverse(expectedLength);

        Assert.Equal(expectedLength[0], totalBytes[0]);
        Assert.Equal(expectedLength[1], totalBytes[1]);
        Assert.Equal(ServerHeartbeatType, (char)totalBytes[2]);
    }
}