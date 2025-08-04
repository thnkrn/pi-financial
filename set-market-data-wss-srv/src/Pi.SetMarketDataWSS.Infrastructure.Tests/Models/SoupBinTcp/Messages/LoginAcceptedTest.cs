using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Tests.Models.SoupBinTcp.Messages
{
    public class LoginAcceptedTests
    {
        private const string Session = "testSessio";
        private const ulong SeqNumber = 1;
        
        [Fact]
        public void Constructor_CreatesCorrectSession_And_SequenceNumber()
        {
            var expectedSession = Session.PadRight(10, ' ');
            var loginAccepted = new LoginAccepted(Session, SeqNumber);

            string actualSession = loginAccepted.Session;
            ulong actualSequenceNumber = loginAccepted.SequenceNumber;

            Assert.Equal(expectedSession, actualSession);
            Assert.Equal(SeqNumber, actualSequenceNumber);
        }

    }
}