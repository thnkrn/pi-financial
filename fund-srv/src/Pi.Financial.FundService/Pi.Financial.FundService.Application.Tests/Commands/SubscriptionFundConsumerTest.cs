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

public class SubscriptionFundConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOptions<FundTradingOptions>> _options;
    private readonly Mock<IMediator> _mediator = new();

    public SubscriptionFundConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _options = new Mock<IOptions<FundTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SubscriptionFundConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOptions<FundTradingOptions>>(_ => _options.Object)
            .AddScoped<IMediator>(_ => _mediator.Object)
            .AddScoped<ILogger<SubscriptionFundConsumer>>(_ => Mock.Of<ILogger<SubscriptionFundConsumer>>())
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });
    }

    [Fact]
    public async Task Should_ResponseExpectedSubscriptionFundOrder_When_Success()
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            Channel = Channel.MOB,
            UnitHolderId = "4343434343",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };

        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "FakeAmcCode",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "4343434343",
            Status = UnitHolderStatus.Normal
        };

        var brokerResponse = new CreateSubscriptionResponse
        {
            TransactionId = "SomeTransactionId",
            SaOrderReferenceNo = "saOrderRefer",
            UnitHolderId = "unitHolder"
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF"));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => brokerResponse);

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(brokerResponse.TransactionId, response.Message.TransactionId);
        Assert.Equal(brokerResponse.UnitHolderId, response.Message.UnitHolderId);
        Assert.Equal(brokerResponse.SaOrderReferenceNo, response.Message.SaOrderReferenceNo);
        Assert.False(response.Message.NewUnitHolder);
    }

    [Fact]
    public async Task Should_ResponseExpectedSubscriptionFundOrder_When_Success_And_UnitHolderExist()
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            UnitHolderId = null,
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };
        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "FakeUnitHolderId",
            Status = UnitHolderStatus.Normal
        };
        var brokerResponse = new CreateSubscriptionResponse
        {
            TransactionId = "SomeTransactionId",
            SaOrderReferenceNo = "saOrderRefer",
            UnitHolderId = unitHolder.UnitHolderId
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A"));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => brokerResponse);

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(unitHolder.UnitHolderId, response.Message.UnitHolderId);
        Assert.Equal(brokerResponse.TransactionId, response.Message.TransactionId);
        Assert.Equal(brokerResponse.UnitHolderId, response.Message.UnitHolderId);
        Assert.Equal(brokerResponse.SaOrderReferenceNo, response.Message.SaOrderReferenceNo);
        Assert.False(response.Message.NewUnitHolder);
    }

    [Theory]
    [InlineData("SSF", UnitHolderType.SEG)]
    [InlineData("RMF", UnitHolderType.SEG)]
    [InlineData("ESG", UnitHolderType.SEG)]
    [InlineData("LTF", UnitHolderType.SEG)]
    [InlineData(null, UnitHolderType.OMN)]
    public async Task Should_ResponseSubscriptionFundOrder_With_ExpectedUnitHolderType_When_Success_And_UnitHolderNotExist(string fundTaxType, UnitHolderType unitHolderType)
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            BankCode = "014",
            TradingAccountNo = "7791109-M",
            Channel = Channel.MOB,
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa,
            UnitHolderId = null,
        };
        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "FakeUnitHolderId",
            Status = UnitHolderStatus.Normal
        };
        var brokerResponse = new CreateSubscriptionResponse
        {
            TransactionId = "SomeTransactionId",
            SaOrderReferenceNo = "saOrderRefer",
            UnitHolderId = "unitHolder"
        };
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF", "RMF", "ESG", "LTF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: fundTaxType));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => brokerResponse);

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(unitHolderType, response.Message.UnitHolderType);
    }

    [Theory]
    [InlineData("SSF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("RMF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("ESG", "7791109-1", UnitHolderType.SEG)]
    [InlineData("LTF", "7791109-1", UnitHolderType.SEG)]
    [InlineData(null, "7791109-1", UnitHolderType.SEG)]
    [InlineData(null, "7791109-M", UnitHolderType.OMN)]
    public async Task Should_ResponseSubscriptionFundOrder_With_ExpectedUnitHolderType_When_Success_And_UnitHolderNotExist_And_AccountHaveSegSuffix(string fundTaxType, string tradingAccountNo, UnitHolderType unitHolderType)
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            UnitHolderId = null,
            Channel = Channel.MOB,
            BankCode = "014",
            TradingAccountNo = tradingAccountNo,
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };
        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.SEG,
            UnitHolderId = "FakeUnitHolderId",
            Status = UnitHolderStatus.Normal
        };
        var brokerResponse = new CreateSubscriptionResponse
        {
            TransactionId = "SomeTransactionId",
            SaOrderReferenceNo = "saOrderRefer",
            UnitHolderId = "unitHolder"
        };
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF", "RMF", "ESG", "LTF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: fundTaxType));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => brokerResponse);

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(unitHolderType, response.Message.UnitHolderType);
    }

    [Theory]
    [InlineData("SSF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("RMF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("ESG", "7791109-1", UnitHolderType.SEG)]
    [InlineData("LTF", "7791109-1", UnitHolderType.SEG)]
    [InlineData(null, "7791109-1", UnitHolderType.SEG)]
    [InlineData(null, "7791109-M", UnitHolderType.OMN)]
    public async Task Should_ResponseExpectedSubscriptionFundOrder_With_ExpectedUnitHolderType_When_Success_And_UnitHolderExist(string fundTaxType, string tradingAccountNo, UnitHolderType expectedUnitHolderType)
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            Channel = Channel.MOB,
            BankAccount = "0112345678",
            UnitHolderId = null,
            BankCode = "014",
            TradingAccountNo = tradingAccountNo,
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };
        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.SEG,
            UnitHolderId = "FakeUnitHolderId",
            Status = UnitHolderStatus.Normal
        };
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF", "RMF", "ESG", "LTF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: fundTaxType));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateSubscriptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                UnitHolderId = "someUnitHolderId"
            });

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(expectedUnitHolderType, response.Message.UnitHolderType);
    }

    [Theory]
    [InlineData("SSF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("RMF", "7791109-1", UnitHolderType.SEG)]
    [InlineData("ESG", "7791109-1", UnitHolderType.SEG)]
    [InlineData("LTF", "7791109-1", UnitHolderType.SEG)]
    [InlineData(null, "7791109-1", UnitHolderType.OMN)]
    [InlineData(null, "7791109-M", UnitHolderType.OMN)]
    public async Task Should_ResponseExpectedSubscriptionFundOrder_With_ExpectedUnitHolderType_When_AccountHadUnitHolders(string fundTaxType, string tradingAccountNo, UnitHolderType expectedUnitHolderType)
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            Channel = Channel.MOB,
            UnitHolderId = null,
            BankCode = "014",
            TradingAccountNo = tradingAccountNo,
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };

        var unitHolder1 = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.SEG,
            UnitHolderId = "dm20240130",
            Status = UnitHolderStatus.Normal
        };

        var unitHolder2 = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "9390104115",
            Status = UnitHolderStatus.Normal
        };

        var unitHolder3 = new CustomerAccountUnitHolder
        {
            AmcCode = "amc",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "9390104116",
            Status = UnitHolderStatus.Normal
        };

        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = 500000,
            SegAccountSuffix = "-1",
            SegTaxTypes = new[] { "SSF", "RMF", "ESG", "LTF" },
            SubscriptionAvoidTaxTypes = new string[1]
        });

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: fundTaxType));
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CustomerAccountDetail
            {
                IcLicense = "123",
                CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder1, unitHolder2, unitHolder3 }
            });
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CreateSubscriptionResponse
            {
                TransactionId = "SomeTransactionId",
                SaOrderReferenceNo = "saOrderRefer",
                UnitHolderId = "someUnitHolderId"
            });

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        Assert.Equal(expectedUnitHolderType, response.Message.UnitHolderType);
    }

    [Fact]
    public async Task Should_FOE003Error_When_FundInfoNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "4343434343",
            Channel = Channel.MOB,
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };

        // Act
        var act = async () => await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions.FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE101.ToString(), code);
    }

    [Theory]
    [InlineData(500000, 500001, 100000)]
    [InlineData(500000, 400001, 600000)]
    [InlineData(500000, 500001, 600000)]
    public async Task Should_FOE003Error_When_FundMinBuyAmountExceedLimit(decimal limit, decimal fstMin, decimal nxtMin)
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptFund>();
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        fundInfo.FirstMinBuyAmount = fstMin;
        fundInfo.NextMinBuyAmount = nxtMin;

        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "123475859",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            UnitHolderId = "4343434343",
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa,
            Channel = Channel.MOB
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _options.Setup(o => o.Value).Returns(new FundTradingOptions
        {
            SaCode = "SaCode",
            MinBuyAmountLimit = limit,
            SegAccountSuffix = "-1",
            SegTaxTypes = new string[1],
            SubscriptionAvoidTaxTypes = new string[1]
        });

        // Act
        var act = async () => await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions.FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE003.ToString(), code);
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
        var client = Harness.GetRequestClient<SubscriptFund>();
        var message = new SubscriptFund
        {
            FundCode = "TNEXTGEN-A",
            EffectiveDate = new DateOnly(),
            BankAccount = "0112345678",
            Channel = Channel.MOB,
            UnitHolderId = "4343434343",
            BankCode = "014",
            TradingAccountNo = "7791109-1",
            Amount = 10,
            CorrelationId = Guid.NewGuid(),
            OrderNo = "FOSUB2024131",
            PaymentType = PaymentType.AtsSa
        };

        var unitHolder = new CustomerAccountUnitHolder
        {
            AmcCode = "FakeAmcCode",
            TradingAccountNo = "FakeTradingAccountNo",
            UnitHolderType = UnitHolderType.OMN,
            UnitHolderId = "4343434343",
            Status = UnitHolderStatus.Normal
        };
        var customerAccountDetail = new CustomerAccountDetail
        {
            IcLicense = "123",
            InvestorClass = investorClass,
            CustomerAccountUnitHolders = new List<CustomerAccountUnitHolder> { unitHolder }
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        fundInfo.ProjectType = projectType;
        var brokerResponse = new CreateSubscriptionResponse
        {
            TransactionId = "SomeTransactionId",
            SaOrderReferenceNo = "saOrderRefer",
            UnitHolderId = "unitHolder"
        };

        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => fundInfo);
        _fundConnextService.Setup(q =>
                q.GetCustomerAccountByAccountNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => customerAccountDetail);
        _fundConnextService.Setup(service =>
                service.CreateSubscriptionAsync(It.IsAny<CreateSubscriptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => brokerResponse);

        // Act
        var act = async () => await client.GetResponse<SubscriptionFundOrder>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()));

        var code = exception.Fault!.Exceptions.FirstOrDefault(q => q.ExceptionType.Equals(typeof(FundOrderException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(FundOrderErrorCode.FOE111.ToString(), code);
    }
}
