namespace Pi.User.Application.Services.DeviceService;

public interface IDeviceService
{
    Task<string> RegisterDevice(string deviceToken);
    Task DeregisterDevice(string deviceIdentifier);
    Task<string> SubscribeTopicTh(string deviceIdentifier);
    Task<string> SubscribeTopicEn(string deviceIdentifier);
    Task<string> UnsubscribeTopic(string subscriptionArn);
}