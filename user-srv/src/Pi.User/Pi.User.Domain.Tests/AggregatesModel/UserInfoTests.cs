using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Domain.Tests.AggregatesModel;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_ShouldBeAbleTo_AddCustCodes_And_UpdateCustCode()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "000");
        var custCodes = new List<string> { "001", "002" };
        userInfo.AddCustCodes(custCodes);
        Assert.Equivalent(custCodes, userInfo.CustCodes.Select(c => c.CustomerCode).ToList());

        var newCustcode = "new01";
        userInfo.UpdateCustCode(newCustcode);
        custCodes.Add(newCustcode);
        Assert.Equivalent(custCodes, userInfo.CustCodes.Select(c => c.CustomerCode).ToList());
    }

    [Fact]
    public void UserInfo_ShouldBeAbleTo_AddTradingAccounts_And_UpdateTradingAccount()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "000");
        var tradingAccIds = new List<string> { "trading-test-01", "trading-test-02" };
        userInfo.AddTradingAccounts(tradingAccIds.Select(x => new TradingAccount(x, "")));
        Assert.Equivalent(tradingAccIds, userInfo.TradingAccounts.Select(t => t.TradingAccountId).ToList());

        var newTradAccId = "new01";
        var newTradAcctCode = "UT";
        userInfo.UpdateTradingAccount(newTradAccId, newTradAcctCode);
        tradingAccIds.Add(newTradAccId);
        Assert.Equivalent(tradingAccIds, userInfo.TradingAccounts.Select(t => t.TradingAccountId).ToList());
    }

    [Fact]
    public void UserInfo_AddDevice_And_Update_Existing_DeviceId()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "000");
        var device = new Device(Guid.NewGuid(), "xxx", "yyy", "000", "ios", "ddd");
        userInfo.AddDevice(device.DeviceId, device.DeviceToken, device.Language, device.DeviceIdentifier, device.Platform, device.SubscriptionIdentifier);
        Assert.Equivalent(device, userInfo.Devices.First());

        var existingDeviceId = userInfo.Devices.First().DeviceId;
        var newDevice = new Device(existingDeviceId, "new", "new", string.Empty, "ios", string.Empty);
        userInfo.UpdateDevice(newDevice.DeviceId, newDevice.DeviceToken, newDevice.Language, device.Platform);
        // Language will be set to empty string instead of new one so that the one who call know that language is changed
        newDevice.UpdateLanguage(string.Empty);
        // Since Subscription is now link to language we will needs to keep the subscription so that we can deregister this
        newDevice.UpdateSubscription("ddd");
        Assert.Equivalent(newDevice, userInfo.Devices.First());
    }

    [Fact]
    public void UserInfo_Should_Generate_Default_Notification_Preference_Correctly_Via_Device_Model()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "000");
        var device = new Device(Guid.NewGuid(), "xxx", "yyy", "000", "ios", "ddd");
        userInfo.AddDevice(device.DeviceId, device.DeviceToken, device.Language, device.DeviceIdentifier, device.Platform, device.SubscriptionIdentifier);

        var defaultNotiPref = new NotificationPreference(true, true, true, true, true);

        Assert.Equivalent(defaultNotiPref, userInfo.Devices.First().NotificationPreference);
    }

    [Fact]
    public void UserInfo_Should_Remove_First_Device_When_Device_Exceed_5_Devices()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "000");
        var devices = new List<Device>();
        for (var i = 0; i < 5; i++)
        {
            var device = new Device(Guid.NewGuid(), $"{i}", "yyy", $"{i}", "ios", $"{i}");
            userInfo.AddDevice(device.DeviceId, device.DeviceToken, device.Language, device.DeviceIdentifier, device.Platform, device.SubscriptionIdentifier);
            devices.Add(device);
        }

        Assert.Equal(5, userInfo.Devices.Count(d => d.IsActive));

        var newDevice = new Device(Guid.NewGuid(), "new", "new", "new", "ios", "ddd");

        userInfo.AddDevice(newDevice.DeviceId, newDevice.DeviceToken, newDevice.Language, newDevice.DeviceIdentifier, newDevice.Platform, newDevice.SubscriptionIdentifier);

        Assert.Equal(5, userInfo.Devices.Count(d => d.IsActive));
        Assert.Equivalent(newDevice, userInfo.Devices.Last(d => d.IsActive));
        Assert.Equivalent(devices[1], userInfo.Devices.First(d => d.IsActive));
    }
}