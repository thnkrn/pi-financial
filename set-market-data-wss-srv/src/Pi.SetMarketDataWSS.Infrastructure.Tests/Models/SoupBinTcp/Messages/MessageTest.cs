using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Tests.Models.SoupBinTcp.Messages;

    public class TestMessage : Message
    {
        public TestMessage(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
    
    public class MessageTests
    {
        [Fact]
        public void Length_ReturnsCorrectLength()
        {
            var message = new TestMessage(new byte[] { 0x01, 0x02, 0x03 });
            Assert.Equal(3, message.Length);
        }

        [Fact]
        public void Type_ReturnsCorrectType()
        {
            var message = new TestMessage(new byte[] { 0x41 });
            Assert.Equal('A', message.Type);
        }

        [Fact]
        public void TotalBytes_IncludesLengthAndDataCorrectly()
        {
            var message = new TestMessage(new byte[] { 0x01, 0x02, 0x03 });
            var totalBytes = message.TotalBytes;
            var expectedLength = BitConverter.GetBytes((ushort)3);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(expectedLength);
            }

            Assert.Equal(expectedLength[0], totalBytes[0]);
            Assert.Equal(expectedLength[1], totalBytes[1]);
            Assert.Equal(0x01, totalBytes[2]);
            Assert.Equal(0x02, totalBytes[3]);
            Assert.Equal(0x03, totalBytes[4]);
        }
    }
