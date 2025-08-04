using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketData.Infrastructure.Tests.Models.SoupBinTcp;

public class LoginDetailsTests
{
    private const string Username = "user1";
    private const string Password = "testPass11";
    private const string Session = "testSessio";
    private const ulong SeqNumber = 1;
    
    [Fact]
    public void LoginDetails_InitializedWithDefaults()
    {
        var loginDetails = new LoginDetails();

        Assert.Null(loginDetails.UserName);
        Assert.Null(loginDetails.Password);
        Assert.Equal(string.Empty, loginDetails.RequestedSession);
        Assert.Equal((ulong)0, loginDetails.RequestedSequenceNumber);
    }

    [Fact]
    public void LoginDetails_InitializedWithValues()
    {
        var loginDetails = new LoginDetails
        {
            UserName = Username,
            Password = Password,
            RequestedSession = Session,
            RequestedSequenceNumber = SeqNumber
        };

        Assert.Equal(Username, loginDetails.UserName);
        Assert.Equal(Password, loginDetails.Password);
        Assert.Equal(Session, loginDetails.RequestedSession);
        Assert.Equal(SeqNumber, loginDetails.RequestedSequenceNumber);
    }
}