using System.Net;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Services.DeviceService;
namespace Pi.User.Infrastructure.Services;

public class AwsService : IDeviceService
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
    private readonly ILogger<AwsService> _logger;
    private readonly string _platformApplicationArn;
    private readonly string _topicThArn;
    private readonly string _topicEnArn;

    public AwsService(IAmazonSimpleNotificationService amazonSimpleNotificationService, IConfiguration configuration, ILogger<AwsService> logger)
    {
        _amazonSimpleNotificationService = amazonSimpleNotificationService;
        _logger = logger;
        _platformApplicationArn = configuration["AwsSns:PlatformApplicationArn"] ?? string.Empty;
        _topicThArn = configuration["AwsSns:TopicArn:Th"] ?? string.Empty;
        _topicEnArn = configuration["AwsSns:TopicArn:En"] ?? string.Empty;
    }

    public async Task<string> RegisterDevice(string deviceToken)
    {
        try
        {
            var createPlatformEndpointResponse = await _amazonSimpleNotificationService.CreatePlatformEndpointAsync(new CreatePlatformEndpointRequest
            {
                Attributes = null,
                CustomUserData = null,
                PlatformApplicationArn = _platformApplicationArn,
                Token = deviceToken
            });

            return createPlatformEndpointResponse.EndpointArn;
        }
        catch (AmazonSimpleNotificationServiceException e)
        {
            _logger.LogError(e, "AwsService. Unable to Register Device. Message: {Message}", e.Message);
            return string.Empty;
        }
    }

    public async Task DeregisterDevice(string deviceIdentifier)
    {
        try
        {
            var deleteEndpointResponse = await _amazonSimpleNotificationService.DeleteEndpointAsync(new DeleteEndpointRequest
            {
                EndpointArn = deviceIdentifier,
            });

            if (deleteEndpointResponse.HttpStatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.InternalServerError)
            {
                _logger.LogError("AwsService. Unable to Deregister Device, RequestId: {RequestId}", deleteEndpointResponse.ResponseMetadata.RequestId);
            }

        }
        catch (AmazonSimpleNotificationServiceException e)
        {
            _logger.LogError(e, "AwsService. Unable to Deregister Device. Message: {Message}", e.Message);
        }
    }

    public async Task<string> SubscribeTopicTh(string deviceIdentifier)
    {
        try
        {
            var resp = await _amazonSimpleNotificationService.SubscribeAsync(
                new SubscribeRequest(
                    _topicThArn,
                    "application",
                    deviceIdentifier)
            );

            return resp.SubscriptionArn;
        }
        catch (AmazonSimpleNotificationServiceException e)
        {
            _logger.LogError(e, "AwsService. Unable to Subscribe Device to Topic TH. Message: {Message}", e.Message);
        }

        return string.Empty;
    }

    public async Task<string> SubscribeTopicEn(string deviceIdentifier)
    {
        try
        {
            var resp = await _amazonSimpleNotificationService.SubscribeAsync(
                new SubscribeRequest(
                    _topicEnArn,
                    "application",
                    deviceIdentifier)
            );

            return resp.SubscriptionArn;
        }
        catch (AmazonSimpleNotificationServiceException e)
        {
            _logger.LogError(e, "AwsService. Unable to Subscribe Device to Topic EN. Message: {Message}", e.Message);
        }

        return string.Empty;
    }

    public async Task<string> UnsubscribeTopic(string subscriptionArn)
    {
        try
        {
            var resp = await _amazonSimpleNotificationService.UnsubscribeAsync(
                new UnsubscribeRequest(subscriptionArn)
            );

            if (resp.HttpStatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.InternalServerError)
            {
                _logger.LogError("AwsService. Unable to Unsubscribe Device, RequestId: {RequestId}", resp.ResponseMetadata.RequestId);
            }
        }
        catch (AmazonSimpleNotificationServiceException e)
        {
            _logger.LogError(e, "AwsService. Unable to Unsubscribe Device to Topic. Message: {Message}", e.Message);
        }

        return string.Empty;
    }
}