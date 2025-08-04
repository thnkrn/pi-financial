using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

public class LoginAcceptedTests
{
    private const string Session = "testSessio";
    private const ulong SeqNumber = 1;

    [Fact]
    public void Constructor_CreatesCorrectSession_And_SequenceNumber()
    {
        var expectedSession = Session.PadRight(10, ' ');
        var loginAccepted = new LoginAccepted(Session, SeqNumber);

        var actualSession = loginAccepted.Session;
        var actualSequenceNumber = loginAccepted.SequenceNumber;

        Assert.Equal(expectedSession, actualSession);
        Assert.Equal(SeqNumber, actualSequenceNumber);
    }
}