using System.Text;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketData.Infrastructure.Tests.Models.SoupBinTcp.Messages
{
    public class ClientHeartbeatTests
    {
        private const char ClientHeartbeatType = 'R';
        
        [Fact]
        public void Constructor_SetsBytesProperty_ToSingleCharR()
        {
            var heartbeat = new ClientHeartbeat();

            var expected = Encoding.ASCII.GetBytes(new[] { 'R' });
            Assert.Equal(expected, heartbeat.Bytes);
        }

        [Fact]
        public void Length_ReturnsCorrectLength_OfBytes()
        {
            var heartbeat = new ClientHeartbeat();

            Assert.Equal(1, heartbeat.Length);
        }

        [Fact]
        public void Type_ReturnsCorrectType_OfMessage()
        {
            var heartbeat = new ClientHeartbeat();

            Assert.Equal(ClientHeartbeatType, heartbeat.Type);
        }

        [Fact]
        public void TotalBytes_IncludesLengthPrefix_AndMessageBytes_ConsideringEndianness()
        {
            var heartbeat = new ClientHeartbeat();

            var totalBytes = heartbeat.TotalBytes;
            var expectedLength = BitConverter.GetBytes((ushort)1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(expectedLength);
            }

            Assert.Equal(expectedLength[0], totalBytes[0]);
            Assert.Equal(expectedLength[1], totalBytes[1]);
            Assert.Equal(ClientHeartbeatType, (char)totalBytes[2]);
        }
    }
}