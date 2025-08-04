

using FluentAssertions;
using Pi.Common.Features;

namespace Pi.User.API.Tests.Controllers;

using MassTransit;
using Pi.User.API.Controllers;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Models;
using Pi.User.Application.Queries;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Device = Pi.User.Application.Models.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class UserDeviceControllerTests : ConsumerTest
{
    private readonly UserDeviceController _controller;
    private readonly Mock<IUserQueries> _userQueriesMock = new();
    private readonly Mock<IFeatureService> _featureServiceMock = new();

    public UserDeviceControllerTests()
    {
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateOrUpdateDeviceConsumer>(); })
            .AddMediator(cfg => { cfg.AddConsumer<CreateOrUpdateDeviceConsumer>(); })
            .AddScoped<IUserQueries>(_ => _userQueriesMock.Object)
            .AddScoped<IFeatureService>(_ => _featureServiceMock.Object)
            .AddScoped<UserDeviceController>()
            .BuildServiceProvider();
        _controller = Provider.GetRequiredService<UserDeviceController>();
    }

    private static User GetCreateOrUpdateDeviceResponse()
    {
        var userMock = new User
        (
            Id: Guid.NewGuid(),
            Devices: new List<Device>(),
            CustomerCodes: new List<CustomerCode>(),
            TradingAccounts: new List<TradingAccount>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        return userMock;
    }

    [Fact]
    public async Task InternalCreateOrUpdateDevice_ReturnErrorResult()
    {
        // Arrange
        var devicePayload = new CreateOrUpdateDevice(Guid.NewGuid(), Guid.NewGuid(), "deviceToken", "th", "ios");
        var apiPayload = new CreateOrUpdateDeviceRequest(devicePayload.DeviceToken);

        // Act
        var result = await _controller.InternalCreateOrUpdateDevice(devicePayload.UserId.ToString(), devicePayload.DeviceId.ToString(), devicePayload.DeviceToken, apiPayload);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ProblemDetails>(problemResult.Value);
        Assert.Equal(ErrorCodes.Usr0000.ToString().ToUpper(), apiResponse.Title);
    }

    [Fact]
    public async Task SecureCreateOrUpdateDevice_WhenSiriusAuthMigrationIsOn_DoesNotPublishSiriusUpdateUserInfoEvent()
    {
        // Arrange
        var devicePayload = new CreateOrUpdateDevice(Guid.NewGuid(), Guid.NewGuid(), "deviceToken", "th", "ios");
        var apiPayload = new CreateOrUpdateDeviceRequest(devicePayload.DeviceToken);
        _featureServiceMock.Setup(fs => fs.IsOff(FeatureSwitch.SiriusAuthMigration)).Returns(false);

        // Act
        await _controller.CreateOrUpdateDevice(devicePayload.UserId.ToString(), devicePayload.DeviceId.ToString(), devicePayload.DeviceToken, "sidMock", apiPayload);

        // Assert
        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType)
            // Fault is caused by amazon sqs context not registered for test, we will just ignore it
            .Where(type => !type?.FullName?.Contains("Fault") ?? true)
            .ToList();

        actualPublishedEvents.Should().BeEquivalentTo([typeof(CreateOrUpdateDevice)]);
    }

    [Fact]
    public async Task SecureCreateOrUpdateDevice_WhenSiriusAuthMigrationIsOn_PublishSiriusUpdateUserInfoEvent()
    {
        // Arrange
        var devicePayload = new CreateOrUpdateDevice(Guid.NewGuid(), Guid.NewGuid(), "deviceToken", "th", "ios");
        var apiPayload = new CreateOrUpdateDeviceRequest(devicePayload.DeviceToken);
        _featureServiceMock.Setup(fs => fs.IsOff(FeatureSwitch.SiriusAuthMigration)).Returns(true);
        UpdateUserInfo expectedUpdateUserInfoRequest = new(Guid.NewGuid());
        List<Type> expectedPublishedEvents = [typeof(UpdateUserInfo), typeof(CreateOrUpdateDevice)];

        // Act
        await _controller.CreateOrUpdateDevice(devicePayload.UserId.ToString(), devicePayload.DeviceId.ToString(), devicePayload.DeviceToken, "sidMock", apiPayload);

        // Assert
        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType)
            // Fault is caused by amazon sqs context not registered for test, we will just ignore it
            .Where(type => !type?.FullName?.Contains("Fault") ?? true)
            .ToList();

        actualPublishedEvents.Should().BeEquivalentTo(expectedPublishedEvents);
    }
}
