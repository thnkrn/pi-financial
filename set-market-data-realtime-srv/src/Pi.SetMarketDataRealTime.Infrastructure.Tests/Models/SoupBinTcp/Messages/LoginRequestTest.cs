using System.Text;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp.Messages;

public class LoginRequestTests
{
    private const string Username = "user1";
    private const string Password = "testPass11";
    private const string Session = "testSessio";
    private const ulong SeqNumber = 1;

    [Fact]
    public void LoginRequest_EncodesAndDecodesCorrectly()
    {
        var loginRequest = new LoginRequest(Username, Password, Session, SeqNumber);

        Assert.Equal(Username.PadRight(6), loginRequest.Username);
        Assert.Equal(Password.PadRight(10), loginRequest.Password);
        Assert.Equal(Session.PadLeft(10), loginRequest.RequestedSession);
        Assert.Equal(SeqNumber, loginRequest.RequestedSequenceNumber);
    }

    [Fact]
    public void LoginRequest_EncodesPropertiesIntoBytesCorrectly()
    {
        var loginRequest = new LoginRequest(Username, Password, Session, SeqNumber);
        var expectedPayload = $"L{Username,-6}{Password,-10}{Session,10}{SeqNumber.ToString(),20}";
        var expectedBytes = Encoding.ASCII.GetBytes(expectedPayload);

        Assert.Equal(expectedBytes, loginRequest.Bytes);
    }
}