using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Services.DeviceService;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Commands;

public record DeregisterDevice(Guid UserId, Guid DeviceId);

public class DeregisterDeviceConsumer : IConsumer<DeregisterDevice>
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DeregisterDeviceConsumer> _logger;
    private readonly IUserInfoRepository _userInfoRepository;

    public DeregisterDeviceConsumer(IUserInfoRepository userInfoRepository, IDeviceService deviceService, ILogger<DeregisterDeviceConsumer> logger)
    {
        _userInfoRepository = userInfoRepository;
        _deviceService = deviceService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeregisterDevice> context)
    {
        var device = await _userInfoRepository.GetByDevice(context.Message.UserId, context.Message.DeviceId);

        if (device == null)
        {
            throw new InvalidDataException($"Unable to Get Device. UserId: ${context.Message.UserId}. DeviceId: ${context.Message.DeviceId}");
        }

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
            _logger.LogError(e, "Unable To Deregister device {DeviceId}", context.Message.DeviceId);
            throw;
        }

        device.MarkInactive();

        await _userInfoRepository.UnitOfWork.SaveChangesAsync();
    }
}