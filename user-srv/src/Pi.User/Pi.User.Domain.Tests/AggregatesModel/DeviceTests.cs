using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Domain.Tests.AggregatesModel;

public class DeviceTests
{
    [Fact]
    public void Device_ShouldBeAbleToUpdateDeviceToken()
    {
        var originalDevice = new Device(Guid.NewGuid(), "ogDeviceToken", "ogLang", "ogIdentifier", "ios", "ddd");
        originalDevice.UpdateDeviceToken("newDeviceToken");
        Assert.Equal("newDeviceToken", originalDevice.DeviceToken);
    }

    [Fact]
    public void Device_ShouldBeAbleToUpdateLanguage()
    {
        var originalDevice = new Device(Guid.NewGuid(), "ogDeviceToken", "ogLang", "ogIdentifier", "ios", "ddd");
        originalDevice.UpdateLanguage("newLang");
        Assert.Equal("newLang", originalDevice.Language);
    }

    [Fact]
    public void Device_ShouldBeAbleToMarkInactive()
    {
        var originalDevice = new Device(Guid.NewGuid(), "ogDeviceToken", "ogLang", "ogIdentifier", "ios", "ddd");
        originalDevice.MarkInactive();
        Assert.False(originalDevice.IsActive);
    }

    [Fact]
    public void Device_ShouldBeAbleToUpdateDeviceIdentifier()
    {
        var originalDevice = new Device(Guid.NewGuid(), "ogDeviceToken", "ogLang", "ogIdentifier", "ios", "ddd");
        originalDevice.UpdateDeviceIdentifier("newIdentifier");
        Assert.Equal("newIdentifier", originalDevice.DeviceIdentifier);
    }

    [Fact]
    public void Device_ShouldBeAbleToUpdateDevicePlatform()
    {
        var originalDevice = new Device(Guid.NewGuid(), "ogDeviceToken", "ogLang", "ogIdentifier", "ios", "ddd");
        originalDevice.UpdateDevicePlatform("android");
        Assert.Equal("android", originalDevice.Platform);
    }
}