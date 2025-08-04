using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.DeviceService;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
namespace Pi.User.Application.Commands;

public record CreateOrUpdateDevice(
    Guid UserId,
    Guid DeviceId,
    string DeviceToken,
    string Language,
    string Platform
)
{
    public string GetDeduplicationId()
        => $"{UserId.ToString().Split("-").Last()}-{DeviceId.ToString().Split("-").Last()}-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
};

public class CreateOrUpdateDeviceConsumer : IConsumer<CreateOrUpdateDevice>
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IDeviceService _deviceService;
    private readonly ILogger<CreateOrUpdateDeviceConsumer> _logger;

    public CreateOrUpdateDeviceConsumer(IUserInfoRepository userInfoRepository, IDeviceService deviceService, ILogger<CreateOrUpdateDeviceConsumer> logger)
    {
        _userInfoRepository = userInfoRepository;
        _deviceService = deviceService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateOrUpdateDevice> context)
    {
        await DeregisterDeviceIfItsBelongToSomeoneElse(context);

        var user = await _userInfoRepository.GetAsync(context.Message.UserId);

        if (user == null)
        {
            throw new InvalidDataException($"UserInfo not found. UserId: {context.Message.UserId}");
        }

        try
        {
            // If DeviceToken Changed || Language Change it will set string.Empty to the field
            user.UpdateDevice(context.Message.DeviceId, context.Message.DeviceToken, context.Message.Language, context.Message.Platform);
            _logger.LogInformation("DEBUG INFO: {UserId}, {DeviceId} {DeviceToken}", context.Message.UserId, context.Message.DeviceId, context.Message.DeviceToken);
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "Multiple Device Detected for {UserId}", context.Message.UserId);
            user.RemoveDuplicateDevice();
        }

        foreach (var device in user.Devices.Where(d => d.IsActive && !string.IsNullOrWhiteSpace(d.DeviceToken)))
        {
            // If Device Identifier or Language is changed we need to Unsubscribe User from Broadcast Notification Subscription Queue 
            if ((string.IsNullOrWhiteSpace(device.DeviceIdentifier) || string.IsNullOrWhiteSpace(device.Language)) && !string.IsNullOrWhiteSpace(device.SubscriptionIdentifier))
            {
                await _deviceService.UnsubscribeTopic(device.SubscriptionIdentifier);
            }

            if (string.IsNullOrWhiteSpace(device.Language))
            {
                device.UpdateLanguage(context.Message.Language);
            }

            if (string.IsNullOrWhiteSpace(device.DeviceIdentifier))
            {
                var deviceIdentifier = await _deviceService.RegisterDevice(device.DeviceToken);

                if (string.IsNullOrWhiteSpace(deviceIdentifier))
                {
                    continue;
                }

                device.UpdateDeviceIdentifier(deviceIdentifier);
            }

            if (!string.IsNullOrWhiteSpace(device.SubscriptionIdentifier) || string.IsNullOrWhiteSpace(device.DeviceIdentifier))
            {
                continue;
            }

            var subscriptionArn = device.Language switch
            {
                "en-US" => await _deviceService.SubscribeTopicEn(device.DeviceIdentifier),
                _ => await _deviceService.SubscribeTopicTh(device.DeviceIdentifier)
            };

            device.UpdateSubscription(subscriptionArn);
        }

        await _userInfoRepository.UnitOfWork.SaveChangesAsync();

        await context.RespondAsync(
            UserQueries.MapUserFromUserInfo(user));
    }

    private async Task DeregisterDeviceIfItsBelongToSomeoneElse(ConsumeContext<CreateOrUpdateDevice> context)
    {
        var devices = (await _userInfoRepository.GetByDeviceId(context.Message.DeviceId))?.ToList();

        if (devices != null && devices.Any())
        {
            foreach (var device in devices.Where(d => d.UserInfoId != null && d.UserInfoId != context.Message.UserId))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(device.DeviceIdentifier))
                    {
                        await _deviceService.DeregisterDevice(device.DeviceIdentifier);
                    }

                    if (!string.IsNullOrWhiteSpace(device.SubscriptionIdentifier))
                    {
                        await _deviceService.UnsubscribeTopic(device.SubscriptionIdentifier);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Unable to Deregister Duplicate Device: {Message}", e.Message);
                }

                device.MarkInactive();
            }
        }

        await _userInfoRepository.UnitOfWork.SaveChangesAsync();
    }
}