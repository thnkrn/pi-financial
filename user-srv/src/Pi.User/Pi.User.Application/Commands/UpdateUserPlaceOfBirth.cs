using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.OnboardService.IntegrationEvents;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.Metrics;

namespace Pi.User.Application.Commands;

public class UpdateUserPlaceOfBirthConsumer : IConsumer<UpdateUserPlaceOfBirthEvent>
{
    private readonly OtelMetrics _metrics;
    private readonly ILogger<UpdateUserPlaceOfBirthConsumer> _logger;
    private readonly IUserInfoRepository _userInfoRepository;

    public UpdateUserPlaceOfBirthConsumer(
        IUserInfoRepository userInfoRepository,
        ILogger<UpdateUserPlaceOfBirthConsumer> logger,
        OtelMetrics metrics)
    {
        _userInfoRepository = userInfoRepository;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task Consume(ConsumeContext<UpdateUserPlaceOfBirthEvent> context)
    {
        try
        {
            var userInfo = await _userInfoRepository.GetAsync(context.Message.UserId) ?? throw new InvalidDataException($"UserInfo not found. UserId: {context.Message.UserId}");

            userInfo.UpdatePlaceOfBirth(context.Message.PlaceOfBirthCountry, context.Message.PlaceOfBirthCity);

            await _userInfoRepository.UnitOfWork.SaveChangesAsync();
            this._metrics.UpdateUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consume UpdateUserPlaceOfBirthEvent: {Message}", ex.Message);
            throw;
        }
    }
}