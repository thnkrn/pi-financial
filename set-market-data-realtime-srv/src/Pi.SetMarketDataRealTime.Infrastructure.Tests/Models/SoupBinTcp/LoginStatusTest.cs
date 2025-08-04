using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;

namespace Pi.SetMarketDataRealTime.Infrastructure.Tests.Models.SoupBinTcp;

public class LoginStatusTests
{
    [Fact]
    public void LoginStatus_SuccessfulLogin()
    {
        var loginStatus = new LoginStatus(true);

        Assert.True(loginStatus.Success);
        Assert.Equal(RejectionReason.NotAuthorised, loginStatus.RejectionReason);
    }

    [Fact]
    public void LoginStatus_UnsuccessfulLogin_DefaultReason()
    {
        var loginStatus = new LoginStatus(false);

        Assert.False(loginStatus.Success);
        Assert.Equal(RejectionReason.NotAuthorised, loginStatus.RejectionReason);
    }

    [Fact]
    public void LoginStatus_UnsuccessfulLogin_SpecificReason()
    {
        var loginStatus = new LoginStatus(false, RejectionReason.SessionNotAvailable);

        Assert.False(loginStatus.Success);
        Assert.Equal(RejectionReason.SessionNotAvailable, loginStatus.RejectionReason);
    }
}