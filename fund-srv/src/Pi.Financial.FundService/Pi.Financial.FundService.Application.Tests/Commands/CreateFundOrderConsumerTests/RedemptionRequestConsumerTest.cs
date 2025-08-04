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

public class RedemptionRequestConsumerTest : ConsumerTest
{
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;

    public RedemptionRequestConsumerTest()
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

    [Fact]
    public async Task Should_PublishRedemptionFundRequestReceived_When_Success()
    {
        // Arrange
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "someSaleLicenseNo",
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

        Assert.True(await Harness.Published.Any<RedemptionFundRequestReceived>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(-1234)]
    public async Task Should_FOE002Error_When_QuantityIsInvalid(decimal quantity)
    {
        // Arrange
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            TradingAccountNo = "7791109-1",
            BankCode = "014",
            Quantity = quantity,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE002.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE003Error_When_FundInfoNotFound()
    {
        // Arrange
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            BankCode = "014",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE102Error_When_UserCustomerCodesMisMatched()
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
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
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE102Error_When_CustomerDetailNotFound()
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A");
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            UnitHolderId = "123456789",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
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
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE105Error_When_Fund_Is_TaxType()
    {
        // Arrange
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "someSaleLicenseNo",
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");

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
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE105.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE112Error_When_Sell_After_SellCutOffTime()
    {
        // Arrange
        var message = new RedemptionRequest
        {
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "123456789",
            Quantity = 10,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "someSaleLicenseNo",
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
        var fault = Harness.Published.Select<Fault<RedemptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE112.ToString(), code);
    }
}
