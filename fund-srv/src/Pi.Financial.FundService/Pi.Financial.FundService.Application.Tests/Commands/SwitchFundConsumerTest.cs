using MassTransit;
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

public class SwitchFundConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IUnitHolderRepository> _unitHolderRepository;

    public SwitchFundConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _unitHolderRepository = new Mock<IUnitHolderRepository>();
        var options = new Mock<IOptions<FundTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SwitchFundConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IUnitHolderRepository>(_ => _unitHolderRepository.Object)
            .AddScoped<IOptions<FundTradingOptions>>(_ => options.Object)
            .AddScoped<ILogger<SwitchFundConsumer>>(_ => Mock.Of<ILogger<SwitchFundConsumer>>())
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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Channel = Channel.MOB,
            Amount = (decimal?)amount,
            UnitHolderId = "123456789",
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = (decimal?)unit,
            RedemptionType = redemptionType,
            SellAllFlag = false,
        };
        var unitHolder = FakeUnitHolder();
        var fundAssets = new List<FundAssetResponse> { FakeFundAssetResponse(message.FundCode, unitHolder.UnitHolderId) };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
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
                service.CreateSwitchAsync(It.IsAny<CreateSwitchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateSwitchResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer"
            });

        // Act
        var response = await client.GetResponse<SwitchingFundOrder>(message);

        // Assert
        Assert.NotNull(response.Message.TransactionId);
        Assert.NotNull(response.Message.SaOrderReferenceNo);
        Assert.NotNull(response.Message.UnitHolderId);
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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Channel = Channel.MOB,
            UnitHolderId = "123456789",
            Amount = redemptionType == RedemptionType.Amount ? (decimal?)quantity : null,
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = redemptionType == RedemptionType.Unit ? (decimal?)quantity : null,
            RedemptionType = redemptionType,
            SellAllFlag = sellAllFlag,
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
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
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
                service.CreateSwitchAsync(It.IsAny<CreateSwitchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateSwitchResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer"
            });

        // Act
        var response = await client.GetResponse<SwitchingFundOrder>(message);

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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            OrderNo = "FOSW20240205",
            Channel = Channel.MOB,
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            TradingAccountNo = "7791109-1",
            BankCode = "014",
            Unit = (decimal?)unit,
            Amount = (decimal?)amount,
            RedemptionType = redemptionType,
            SellAllFlag = false,
            CounterFundCode = "TNEXTGEN-A",
        };

        // Act
        var act = async () => await client.GetResponse<CreateSwitchRequest>(message);

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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            OrderNo = "FOSW20240205",
            Amount = 100,
            FundCode = "TNEXTGEN-A",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            TradingAccountNo = "7791109-1",
            CounterFundCode = "TNEXTGEN-A",
            BankCode = "014",
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        var act = async () => await client.GetResponse<SwitchingFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE106Failed_When_UnitHolderNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            UnitHolderId = "123456789",
            Amount = null,
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            SellAllFlag = false,
        };
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode),
        };

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder>()
            });

        // Act
        var act = async () => await client.GetResponse<SwitchingFundOrder>(message);

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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            UnitHolderId = "123456789",
            Amount = null,
            Channel = Channel.MOB,
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            SellAllFlag = false,
        };
        var unitHolder = FakeUnitHolder();
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode, "Mismatched"),
            FakeFundAssetResponse("MISMATCHED", unitHolder.UnitHolderId),
        };

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
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
        var act = async () => await client.GetResponse<SwitchingFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE107.ToString(), code);
    }

    [Fact]
    public async Task Should_FOE103Failed_When_SaleLicenseNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            UnitHolderId = "123456789",
            Amount = null,
            Channel = Channel.MOB,
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10,
            RedemptionType = RedemptionType.Unit,
            SellAllFlag = false,
        };
        var unitHolder = FakeUnitHolder();
        var fundInfo = FakeFactory.NewFundInfo(message.FundCode);
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);
        var fundAssets = new List<FundAssetResponse>
        {
            FakeFundAssetResponse(message.FundCode, "123456789"),
        };

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);

        // Act
        var act = async () => await client.GetResponse<SwitchingFundOrder>(message);

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
        var client = Harness.GetRequestClient<SwitchingFund>();
        var message = new SwitchingFund
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSW20240205",
            Channel = Channel.MOB,
            Amount = 10.03m,
            UnitHolderId = "123456789",
            FundCode = "TNEXTGEN-A",
            CounterFundCode = "TNEXTGEN-B",
            BankCode = "014",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            TradingAccountNo = "7791109-1",
            Unit = 10.06m,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
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
        var counterFundInfo = FakeFactory.NewFundInfo(message.CounterFundCode);

        _marketService.Setup(service =>
                service.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new[] { fundInfo, counterFundInfo });
        _fundConnextService.Setup(service =>
                service.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => customerAccountDetail);
        _fundConnextService.Setup(service =>
                service.CreateSwitchAsync(It.IsAny<CreateSwitchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateSwitchResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer"
            });

        // Act
        var act = async () => await client.GetResponse<SwitchingFundOrder>(message);

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

    private static CustomerAccountUnitHolder FakeUnitHolder()
    {
        return new CustomerAccountUnitHolder
        {
            TradingAccountNo = "FakeTradingAccountNo",
            AmcCode = "FakeAmcCode",
            UnitHolderId = "123456789",
            UnitHolderType = UnitHolderType.OMN,
            Status = UnitHolderStatus.Normal
        };
    }
}
