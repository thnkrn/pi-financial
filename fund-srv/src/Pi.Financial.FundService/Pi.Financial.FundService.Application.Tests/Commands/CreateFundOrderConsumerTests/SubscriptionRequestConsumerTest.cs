using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Options;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.Metric;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands.CreateFundOrderConsumerTests;

public class SubscriptionRequestConsumerTest : ConsumerTest
{
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IOptions<FundTradingOptions>> _options;

    public SubscriptionRequestConsumerTest()
    {
        _marketService = new Mock<IMarketService>();
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _options = new Mock<IOptions<FundTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateFundOrderConsumer>(); })
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IMetricService>(_ => new Mock<IMetricService>().Object)
            .AddScoped<IOptions<FundTradingOptions>>(_ => _options.Object)
            .AddScoped<ILogger<CreateFundOrderConsumer>>(_ => Mock.Of<ILogger<CreateFundOrderConsumer>>())
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF" },
            SubscriptionAvoidTaxTypes = new[] { "LTF" }
        });
    }

    [Fact]
    public async Task Should_PublishFundOrderRequestReceived_When_Success()
    {
        // Arrange
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
            BankAccount = "0112345678",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            UnitHolderId = "4343434343",
            PaymentMethod = PaymentMethod.Ats,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "someSale"
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        fundInfo.PiBuyCutOffTime = DateTime.UtcNow.AddMinutes(30);

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF"));
        _onboardService.Setup(service => service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tradingAccount);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<FundOrderRequestReceived>());
    }

    [Fact]
    public async Task Should_FOE001Error_When_PaymentTypeIsBankTransfer()
    {
        // Arrange
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "4343434343",
            Quantity = 100,
            PaymentMethod = PaymentMethod.BankTransfer,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE001.ToString(), code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(-1234)]
    public async Task Should_FOE002Error_When_QuantityIsInvalid(decimal quantity)
    {
        // Arrange
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "4343434343",
            Quantity = quantity,
            PaymentMethod = PaymentMethod.Ats,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE002.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE003Error_When_FundInfoNotFound()
    {
        // Arrange
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "4343434343",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Theory]
    [InlineData("7711431-2")]
    [InlineData("7711444-S")]
    [InlineData("7711440-10")]
    public async Task Should_FOE102Error_When_TradingAccountInvalid(string tradingAccountNo)
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = tradingAccountNo,
            UnitHolderId = "4343434343",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE102Error_When_UserCustomerCodesMisMatched()
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            UnitHolderId = "4343434343",
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
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
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
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            UnitHolderId = "4343434343",
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
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE102.ToString(), code);
    }

    [Theory]
    [InlineData(500000, 500001, 100000)]
    [InlineData(500000, 400001, 600000)]
    [InlineData(500000, 500001, 600000)]
    public async Task Should_FOE003Error_When_FundMinBuyAmountExceedLimit(decimal limit, decimal fstMin, decimal nxtMin)
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        fundInfo.FirstMinBuyAmount = fstMin;
        fundInfo.NextMinBuyAmount = nxtMin;
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            UnitHolderId = "4343434343",
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = limit,
            SegAccountSuffix = "-1",
            SegTaxTypes = new string[1],
            SubscriptionAvoidTaxTypes = new string[1]
        });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE003.ToString(), code);
    }

    [Theory]
    [InlineData("SSFX")]
    [InlineData("LTF")]
    public async Task Should_FOE110Error_When_Fund_TaxTypes_Cannot_Buy(string taxType)
    {
        // Arrange
        var fundInfo = FakeFactory.NewFundInfo("K-SUPSTAR-SSFX", taxType: taxType);
        var message = new SubscriptionRequest
        {
            FundCode = "K-SUPSTAR-SSFX",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            PaymentMethod = PaymentMethod.Ats,
            UnitHolderId = "4343434343",
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500,
            SegAccountSuffix = "-1",
            SegTaxTypes = new string[1],
            SubscriptionAvoidTaxTypes = new[] { taxType }
        });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE110.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE112Error_When_Buy_After_BuyCutOffTime()
    {
        // Arrange
        var message = new SubscriptionRequest
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow),
            BankAccount = "0112345678",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Quantity = 10,
            UnitHolderId = "4343434343",
            PaymentMethod = PaymentMethod.Ats,
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
        var tradingAccount = new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "someSale"
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        fundInfo.PiBuyCutOffTime = DateTime.UtcNow.AddMinutes(-30);

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _onboardService.Setup(service => service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => tradingAccount);
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { message.TradingAccountNo.Replace("-1", "") });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<SubscriptionRequest>>()
            .FirstOrDefault(q => q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];

        Assert.Equal(FundOrderErrorCode.FOE112.ToString(), code);
    }
}
