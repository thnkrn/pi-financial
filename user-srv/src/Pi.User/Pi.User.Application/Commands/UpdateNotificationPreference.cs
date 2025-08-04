using MassTransit;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Commands;

public record UpdateNotificationPreference(
    Guid UserId,
    Guid DeviceId,
    bool? Important,
    bool? Order,
    bool? Portfolio,
    bool? Wallet,
    bool? Market);

public class UpdateNotificationPreferenceConsumer : IConsumer<UpdateNotificationPreference>
{
    private readonly IUserInfoRepository _userInfoRepository;


    public UpdateNotificationPreferenceConsumer(IUserInfoRepository userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }

    public async Task Consume(ConsumeContext<UpdateNotificationPreference> context)
    {
        var device = await _userInfoRepository.GetByDevice(context.Message.UserId, context.Message.DeviceId);

        if (device == null)
        {
            throw new InvalidOperationException($"Device not found. DeviceId: {context.Message.DeviceId}");
        }

        device.NotificationPreference.Update(
            context.Message.Important,
            context.Message.Order,
            context.Message.Portfolio,
            context.Message.Wallet,
            context.Message.Market);

        await _userInfoRepository.UnitOfWork.SaveChangesAsync();
    }
}