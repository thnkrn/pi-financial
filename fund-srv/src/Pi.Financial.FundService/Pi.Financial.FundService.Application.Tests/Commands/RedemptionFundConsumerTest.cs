using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Options;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class RedemptionFundConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IMediator> _mediator = new();

    public RedemptionFundConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        var options = new Mock<IOptions<FundTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<RedemptionFundConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOptions<FundTradingOptions>>(_ => options.Object)
            .AddScoped<IMediator>(_ => _mediator.Object)
            .AddScoped<ILogger<RedemptionFundConsumer>>(_ => Mock.Of<ILogger<RedemptionFundConsumer>>())
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

    [Theory]
    [InlineData(null, 10.06, RedemptionType.Unit)]
    [InlineData(10.03, null, RedemptionType.Amount)]
    [InlineData(10.03, 10.06, RedemptionType.Unit)]
    [InlineData(10.03, 10.06, RedemptionType.Amount)]
    public async Task Should_Success_With_Expected(double? amount, double? unit, RedemptionType redemptionType)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = (decimal?)amount,
            FundCode = "TNEXTgen-A",
            BankCode = "014",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = (decimal?)unit,
            RedemptionType = redemptionType,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var unitHolder = FakeUnitHolder();
        var fundAssets = new List<FundAssetResponse> { FakeFundAssetResponse(message.FundCode, unitHolder.UnitHolderId) };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateRedemptionAsync(It.IsAny<CreateRedemptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateRedemptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                SettlementDate = new DateOnly()
            });

        // Act
        var response = await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        Assert.NotNull(response.Message.TransactionId);
        Assert.NotNull(response.Message.SaOrderReferenceNo);
        Assert.NotNull(response.Message.UnitHolderId);
    }

    [Theory]
    [InlineData(null, UnitHolderType.OMN, PaymentType.AtsSa)]
    [InlineData("SSF", UnitHolderType.SEG, PaymentType.AtsAmc)]
    public async Task Should_Success_With_ExpectedPaymentType(string? fundTaxType, UnitHolderType unitHolderType, PaymentType expectedPaymentType)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            UnitHolderId = "123456789",
            Amount = null,
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10.03m,
            RedemptionType = RedemptionType.Unit,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var unitHolder = FakeUnitHolder(unitHolderType);
        var fundAssets = new List<FundAssetResponse> { FakeFundAssetResponse(message.FundCode, unitHolder.UnitHolderId) };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode, taxType: fundTaxType);

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateRedemptionAsync(It.IsAny<CreateRedemptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateRedemptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                SettlementDate = new DateOnly()
            });

        // Act
        var response = await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        Assert.Equal(expectedPaymentType, response.Message.PaymentType);
    }

    [Theory]
    [InlineData(true, RedemptionType.Amount, 10, 30, false)]
    [InlineData(false, RedemptionType.Amount, 10, 30, false)]
    [InlineData(null, RedemptionType.Amount, 10, 30, false)]
    [InlineData(null, RedemptionType.Amount, 30, 30, false)]
    [InlineData(true, RedemptionType.Unit, 10, 30, true)]
    [InlineData(false, RedemptionType.Unit, 10, 30, false)]
    [InlineData(null, RedemptionType.Unit, 10, 30, false)]
    [InlineData(null, RedemptionType.Unit, 30, 30, true)]
    public async Task Should_Success_With_ExpectedSellAllFlag(bool? sellAllFlag, RedemptionType redemptionType, double quantity, double remainQuantity, bool expected)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            UnitHolderId = "123456789",
            Amount = redemptionType == RedemptionType.Amount ? (decimal?)quantity : null,
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = redemptionType == RedemptionType.Unit ? (decimal?)quantity : null,
            RedemptionType = redemptionType,
            SellAllFlag = sellAllFlag,
            Channel = Channel.MOB,
        };
        var unitHolder = FakeUnitHolder();
        var fundAsset = new FundAssetResponse()
        {
            UnitholderId = unitHolder.UnitHolderId,
            FundCode = message.FundCode,
            Unit = 10.123m,
            Amount = 400.50m,
            RemainUnit = redemptionType == RedemptionType.Unit ? (decimal)remainQuantity : 0,
            RemainAmount = redemptionType == RedemptionType.Amount ? (decimal)remainQuantity : 0,
            PendingAmount = 0,
            PendingUnit = 0,
            AvgCost = 10.0m,
            Nav = 11.345m,
            NavDate = new DateOnly()
        };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse> { fundAsset });
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateRedemptionAsync(It.IsAny<CreateRedemptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateRedemptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                SettlementDate = new DateOnly()
            });

        // Act
        var response = await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        Assert.Equal(expected, response.Message.SellAllFlag);
    }

    [Theory]
    [InlineData(null, 10.06, RedemptionType.Amount)]
    [InlineData(10.03, null, RedemptionType.Unit)]
    [InlineData(null, -10.33, RedemptionType.Amount)]
    [InlineData(-10.33, null, RedemptionType.Unit)]
    [InlineData(-10.33, 10.06, RedemptionType.Amount)]
    [InlineData(10.33, -10.06, RedemptionType.Unit)]
    public async Task Should_FOE002Error_When_UnitOrAmountInvalid(double? amount, double? unit, RedemptionType redemptionType)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            UnitHolderId = "123456789",
            TradingAccountNo = "7791109-1",
            BankCode = "014",
            Unit = (decimal?)unit,
            Amount = (decimal?)amount,
            RedemptionType = redemptionType,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };

        // Act
        var act = async () => await client.GetResponse<CreateRedemptionRequest>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions.FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE002.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE101Error_When_FundInfoNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = 100,
            FundCode = "TNEXTGEN-A",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            TradingAccountNo = "7791109-1",
            BankCode = "014",
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Theory]
    [InlineData(null, UnitHolderType.SEG)]
    [InlineData("SSF", UnitHolderType.OMN)]
    [InlineData("ABC", UnitHolderType.SEG)]
    public async Task Should_FOE106Failed_When_UnitHolderNotFound(string? fundTaxType, UnitHolderType unitHolderType)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = null,
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            UnitHolderId = "123456789",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode, taxType: fundTaxType);
        var unitHolder = FakeUnitHolder(unitHolderType, "8676676767");

        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode),
        };
        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE106.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE107Failed_When_FundAssetNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = null,
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            UnitHolderId = "123456789",
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse("MISMATCHED", "6646465564"),
        };

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE107.ToString(), code);
    }

    [Theory]
    [InlineData(999999.99, null, RedemptionType.Amount)]
    [InlineData(null, 999999.99, RedemptionType.Unit)]
    public async Task Should_FOE004Failed_When_PayloadQuantityGreaterThanRemainQuantity(double? amount, double? unit, RedemptionType redemptionType)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = (decimal?)unit,
            Amount = (decimal?)amount,
            RedemptionType = redemptionType,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var unitHolder = FakeUnitHolder();
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode, unitHolder.UnitHolderId),
        };
        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE004.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE103Error_When_SaleLicenseNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = null,
            FundCode = "TNEXTGEN-A",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            UnitHolderId = "123456789",
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode),
        };

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { }
            });

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE103.ToString(), code);
    }

    [Theory]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.Retail)]
    [InlineData(FundProjectType.NonRetailInvestorsFund, InvestorClass.Institutional)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.Retail)]
    [InlineData(FundProjectType.NonRetailAndHighNetWorthInvestorsFund, InvestorClass.Institutional)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.Retail)]
    [InlineData(FundProjectType.HighNetWorthInvestorsFund, InvestorClass.Institutional)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.HighNetWorth)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.Retail)]
    [InlineData(FundProjectType.InstitutionalAndSpecialLargeInvestorsFund, InvestorClass.Institutional)]
    public async Task Should_FOE111Error_When_Customer_Cannot_Access_NonRetail_Fund(FundProjectType projectType, InvestorClass investorClass)
    {
        // Arrange
        var client = Harness.GetRequestClient<RedeemFund>();
        var message = new RedeemFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Amount = 10.03m,
            FundCode = "TNEXTgen-A",
            BankCode = "014",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10.06m,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
            Channel = Channel.MOB,
        };
        var unitHolder = FakeUnitHolder();
        var customerAccountDetail = new CustomerAccountDetail
        {
            IcLicense = "123",
            InvestorClass = investorClass,
            CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
        };
        var fundAssets = new List<FundAssetResponse> { FakeFundAssetResponse(message.FundCode, unitHolder.UnitHolderId) };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        fundInfo.ProjectType = projectType;

        _marketService.Setup(service =>
                service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => customerAccountDetail);
        _fundConnextService.Setup(service =>
                service.CreateRedemptionAsync(It.IsAny<CreateRedemptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateRedemptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                SettlementDate = new DateOnly()
            });

        // Act
        var act = async () => await client.GetResponse<RedemptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE111.ToString(), code);
    }

    private static FundAssetResponse FakeFundAssetResponse(string? fundCode = null, string? unitHolderId = null)
    {
        return new()
        {
            UnitholderId = unitHolderId ?? "123456789",
            FundCode = fundCode ?? "F-FUND-123",
            Unit = 10.123m,
            Amount = 400.50m,
            RemainUnit = 15.0m,
            RemainAmount = 200.25m,
            PendingAmount = 0,
            PendingUnit = 0,
            AvgCost = 10.0m,
            Nav = 11.345m,
            NavDate = new DateOnly()
        };
    }

    private static CustomerAccountUnitHolder FakeUnitHolder(UnitHolderType? unitHolderType = null, string? unitHolderId = null)
    {
        return new CustomerAccountUnitHolder
        {
            TradingAccountNo = "FakeTradingAccountNo",
            AmcCode = "FakeAmcCode",
            UnitHolderId = unitHolderId ?? "123456789",
            UnitHolderType = unitHolderType ?? UnitHolderType.OMN,
            Status = UnitHolderStatus.Normal
        };
    }
}
