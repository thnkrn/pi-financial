using Pi.Client.OnboardService.Api;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;

namespace Pi.StructureNotes.Infrastructure.Tests.Services;

public class AccountServiceTest
{
    [Fact(Skip = "For instant testing only")]
    private async Task GetUserId_Test()
    {
        ITradingAccountApi _tradingAccApi =
            new TradingAccountApi(new HttpClient(), "http://onboard-api.nonprod.pi.internal");
        IUserApi _userApi = new UserApi(new HttpClient(), "http://user.nonprod.pi.internal");

        string custCode = "7711247";
        //var custCode = "0175643";

        PiUserAPIModelsUserInfoResponseApiResponse response = await _userApi.InternalUserIdGetAsync(custCode, true);

        Assert.NotNull(response);

        Guid userId = response.Data.Id;

        Assert.True(userId != default); //7ebf8578-06af-4d1b-a614-baa8de9e58aa
    }
}
