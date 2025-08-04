using Pi.User.Application.Models;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Device = Pi.User.Application.Models.Device;
using NotificationPreference = Pi.User.Application.Models.NotificationPreference;

namespace Pi.User.Application.Queries;

public class NotificationPreferenceQueries : INotificationPreferenceQueries
{
    private readonly IUserInfoRepository _userInfoRepository;

    public NotificationPreferenceQueries(IUserInfoRepository userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<Device> GetNotificationPreference(Guid userId, Guid deviceId)
    {
        var res = await _userInfoRepository.GetByDevice(userId, deviceId);

        if (res == null)
        {
            throw new InvalidDataException($"Device not found. UserId: {userId} DeviceId: {deviceId}");
        }

        return new Device(
            res.DeviceId,
            res.DeviceToken,
            res.DeviceIdentifier,
            res.Language,
            res.Platform,
            new NotificationPreference(
                res.NotificationPreference.Important,
                res.NotificationPreference.Order,
                res.NotificationPreference.Portfolio,
                res.NotificationPreference.Wallet,
                res.NotificationPreference.Market)
        );
    }
}