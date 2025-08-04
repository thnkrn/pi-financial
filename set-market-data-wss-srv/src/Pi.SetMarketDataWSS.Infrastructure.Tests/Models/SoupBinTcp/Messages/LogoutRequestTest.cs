using System.Text;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Tests.Models.SoupBinTcp.Messages
{
    public class LogoutRequestTests
    {
        private const char LogoutRequestType = 'O';
        
        [Fact]
        public void Constructor_SetsBytesProperty_ToTypeIndicatorO()
        {
            var logoutRequest = new LogoutRequest();

            var expected = Encoding.ASCII.GetBytes(new[] { LogoutRequestType });
            Assert.Equal(expected, logoutRequest.Bytes);
        }

        [Fact]
        public void Type_ReturnsCorrectType_OfMessage()
        {
            var logoutRequest = new LogoutRequest();

            Assert.Equal(LogoutRequestType, logoutRequest.Type);
        }

        [Fact]
        public void Length_ReturnsCorrectLength_OfBytes()
        {
            var logoutRequest = new LogoutRequest();

            Assert.Equal(1, logoutRequest.Length);
        }

        [Fact]
        public void TotalBytes_IncludesLengthPrefix_AndMessageBytes_ConsideringEndianness()
        {
            var logoutRequest = new LogoutRequest();

            var totalBytes = logoutRequest.TotalBytes;
            var expectedLength = BitConverter.GetBytes((ushort)1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(expectedLength);
            }

            Assert.Equal(expectedLength[0], totalBytes[0]);
            Assert.Equal(expectedLength[1], totalBytes[1]);
            Assert.Equal(LogoutRequestType, (char)totalBytes[2]);
        }
    }
}