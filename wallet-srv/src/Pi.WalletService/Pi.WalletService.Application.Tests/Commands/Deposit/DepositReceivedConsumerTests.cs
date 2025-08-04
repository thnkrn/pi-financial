using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using ValidationResult = Pi.WalletService.Application.Services.ValidationResult;

namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class DepositReceivedConsumerTests : ConsumerTest
{
    private readonly Mock<IUserService> _userService;

    public DepositReceivedConsumerTests()
    {
        _userService = new Mock<IUserService>();
        Mock<IOnboardService> onboardService = new();
        Mock<IWalletQueries> walletQueries = new();
        Mock<IOptionsSnapshot<QrCodeOptions>> qrCodeOptions = new();
        Mock<IOptionsSnapshot<FeaturesOptions>> featuresOptions = new();
        Mock<ILogger<RequestDeposit>> logger = new();
        Mock<IFeatureService> featureService = new();
        qrCodeOptions.Setup(q => q.Value).Returns(new QrCodeOptions
        {
            TimeOutMinute = 60
        });
        featuresOptions.Setup(q => q.Value).Returns(new FeaturesOptions
        {
            FreewillClosingTime = "23:59"
        });
        var validationService = new Mock<IValidationService>();
        validationService.Setup(v =>
            v.IsOutsideWorkingHour(It.IsAny<Product>(),
                It.IsAny<Channel>(),
                It.IsAny<DateTime>(),
                out It.Ref<ValidationResult>.IsAny)).Returns(false);
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<RequestDepositConsumer>(); })
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IOnboardService>(_ => onboardService.Object)
            .AddScoped<IWalletQueries>(_ => walletQueries.Object)
            .AddScoped<IFeatureService>(_ => featureService.Object)
            .AddScoped<IOptionsSnapshot<QrCodeOptions>>(_ => qrCodeOptions.Object)
            .AddScoped<IValidationService>(_ => validationService.Object)
            .AddScoped<IOptionsSnapshot<FeaturesOptions>>(_ => featuresOptions.Object)
            .AddScoped<ILogger<RequestDeposit>>(_ => logger.Object)
            .BuildServiceProvider(true);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Able_To_Get_All_Data_And_Publish_DepositRequestReceived_Event()
    {
        // Arrange
        const string accountCode = "77114962";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>
                {
                    "7711496"
                },
                new List<string>
                {
                    accountCode
                },
                "Unit",
                "Test",
                "Unit",
                "Test",
                string.Empty,
                string.Empty,
                string.Empty));

        // Act
        await Harness.Bus.Publish(
            new RequestDeposit(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                "7711496",
                Product.GlobalEquities,
                Channel.QR,
                2000,
                Guid.NewGuid(),
                null,
                false,
                123456,
                "THB",
                "123456",
                23,
                "USD"
            ));

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestDeposit>());
        Assert.True(await Harness.Published.Any<DepositRequestReceived>());

        var response = Harness.Published.Select<DepositRequestReceived>().First().Context;
        Assert.Equal(accountCode, response.Message.AccountCode);
        Assert.Equal("Unit Test", response.Message.CustomerThaiName);
        Assert.Equal("Unit Test", response.Message.CustomerEnglishName);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Able_To_Get_All_Data_And_Publish_DepositRequestReceived_Event_Non_Global()
    {
        // Arrange
        const string accountCode = "77114961";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>
                {
                    "7711496"
                },
                new List<string>
                {
                    accountCode
                },
                "Unit",
                "Test",
                "Unit",
                "Test",
                string.Empty,
                string.Empty,
                string.Empty));

        // Act
        await Harness.Bus.Publish(
            new RequestDeposit(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                "7711496",
                Product.Cash,
                Channel.QR,
                2000,
                Guid.NewGuid(),
                null,
                false
            ));

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestDeposit>());
        Assert.True(await Harness.Published.Any<DepositRequestReceived>());

        var response = Harness.Published.Select<DepositRequestReceived>().First().Context;
        Assert.Equal(accountCode, response.Message.AccountCode);
        Assert.Equal("Unit Test", response.Message.CustomerThaiName);
        Assert.Equal("Unit Test", response.Message.CustomerEnglishName);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Throw_Exception_When_Trading_Account_Not_Found()
    {
        // Arrange
        const string accountCode = "77114969";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>
                {
                    "7711496"
                },
                new List<string>
                {
                    accountCode
                },
                "Unit",
                "Test",
                "Unit",
                "Test",
                string.Empty,
                string.Empty,
                string.Empty));

        // Act
        var client = Harness.GetRequestClient<RequestDeposit>();
        var response = await client.GetResponse<BusRequestFailed>(
            new RequestDeposit(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                "7711496",
                Product.Cash,
                Channel.QR,
                2000,
                Guid.NewGuid(),
                null,
                false
            ));

        // assert
        Assert.True(await Harness.Consumed.Any<RequestDeposit>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }
}
