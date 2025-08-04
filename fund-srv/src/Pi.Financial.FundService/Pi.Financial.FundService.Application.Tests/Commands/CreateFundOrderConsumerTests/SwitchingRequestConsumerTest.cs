using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Options;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.Metric;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands.CreateFundOrderConsumerTests;

public class SwitchingRequestConsumerTest : ConsumerTest
{
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;

    public SwitchingRequestConsumerTest()
    {
        _marketService = new Mock<IMarketService>();
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        var options = new Mock<IOptions<FundTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateFundOrderConsumer>(); })
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IMetricService>(_ => new Mock<IMetricService>().Object)
            .AddScoped<IOptions<FundTradingOptions>>(_ => options.Object)
            .AddScoped<ILogger<CreateFundOrderConsumer>>(_ => Mock.Of<ILogger<CreateFundOrderConsumer>>())
            .BuildServiceProvider(true);

        options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_PublishSwitchingFundRequestReceived_When_Success()
    {
        // Arrange
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "saleLicense",
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        fundInfo.PiSellCutOffTime = DateTime.UtcNow.AddMinutes(30);

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tradingAccount);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert

        Assert.True(await Harness.Published.Any<SwitchingFundRequestReceived>());
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_PublishSwitchingFundRequestReceived_When_Success_And_FundIsTaxType()
    {
        // Arrange

        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "saleLicense",
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-ASSF", "SSF");
        fundInfo.PiSellCutOffTime = DateTime.UtcNow.AddMinutes(30);

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tradingAccount);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert

        Assert.True(await Harness.Published.Any<SwitchingFundRequestReceived>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(-1234)]
    public async Task Should_FOE002Error_When_QuantityIsInvalid(decimal quantity)
    {
        // Arrange
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            TradingAccountNo = "7791109-1",
            Quantity = quantity,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SwitchingRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE002.ToString(), code);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_FOE003Error_When_FundInfoNotFound()
    {
        // Arrange
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SwitchingRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_FOE102Error_When_UserCustomerCodesMisMatched()
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { "8899110" });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SwitchingRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_FOE102Error_When_CustomerDetailNotFound()
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SwitchingRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_FOE112Error_When_Switch_After_SellCutOffTime()
    {
        // Arrange
        var message = new SwitchingRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            CounterFundCode = "KF-OIL",
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "SaleLicense",
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        fundInfo.PiSellCutOffTime = DateTime.UtcNow.AddMinutes(-30);

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tradingAccount);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SwitchingRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE112.ToString(), code);
    }
}
