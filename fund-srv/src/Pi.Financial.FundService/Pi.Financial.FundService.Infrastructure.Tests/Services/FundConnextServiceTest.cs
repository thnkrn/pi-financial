using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Moq;
using Pi.Common.Features;
using Pi.Financial.Client.FundConnext.Api;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.Services;
using Serilog;
using FundAsset = Pi.Financial.Client.FundConnext.Model.FundAsset;
using FundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Infrastructure.Tests.Services;

public class FundConnextServiceTest
{
    private readonly Mock<IFundConnextApi> _fundConnextApi;
    private readonly Mock<IAccountOpeningV5Api> _accountOpeningV5Api;
    private readonly Mock<IDistributedCache> _distributedCache;
    private readonly FundConnextService _fundConnextService;

    public FundConnextServiceTest()
    {
        _fundConnextApi = new Mock<IFundConnextApi>();
        _accountOpeningV5Api = new Mock<IAccountOpeningV5Api>();
        _distributedCache = new Mock<IDistributedCache>();
        var logger = new Mock<ILogger>();
        var mockSection = new Mock<IConfigurationSection>();
        var mockConfig = new Mock<IConfiguration>();
        mockSection.Setup(x => x.Value).Returns("30");
        mockConfig.Setup(x => x.GetSection("FundConnext:TokenCacheExpiration")).Returns(mockSection.Object);

        _fundConnextService = new FundConnextService(_fundConnextApi.Object, _accountOpeningV5Api.Object, _distributedCache.Object, mockConfig.Object, logger.Object);
    }

    [Fact]
    public async Task Should_Return_FundOrders_When_GetAccountFundOrdersAsync()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.Setup(q => q.ApiAccountFundOrdersV2GetAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountFundOrdersV2Get200Response(1, 1, false, new List<FundOrder>() { FakeFundOrder() }));

        // Act
        var actual = await _fundConnextService.GetAccountFundOrdersAsync("accountNo", new DateOnly(), new DateOnly());

        // Assert
        Assert.IsType<List<Application.Models.FundOrder>>(actual);
        Assert.Single(actual);
    }

    [Fact]
    public async Task Should_Delete_SubscriptionFundOrder_When_ApiSubscriptionsTransactionIdDeleteAsync_Executed()
    {
        // Arrange
        const string transactionId = "1672312180000831";
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.Setup(q => q.ApiSubscriptionsTransactionIdDeleteAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ApiSubscriptionsTransactionIdDeleteRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSubscriptionsTransactionIdDelete200Response(transactionId));

        // Act
        var actual = await _fundConnextService.CancelSubscriptionOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672312180000831",
            OrderSide = OrderSide.Buy,
            Force = true
        }, It.IsAny<CancellationToken>());

        // Assert
        Assert.IsType<CancelFundOrderResponse>(actual);
        Assert.Equal(transactionId, actual.TransactionId);
    }

    [Fact]
    public async Task Should_Delete_SwitchingFundOrder_When_ApiSwitchingTransactionIdDeleteAsync_Executed()
    {
        // Arrange
        const string transactionId = "1672312180000831";
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.Setup(q => q.ApiSwitchingsTransactionIdDeleteAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ApiSubscriptionsTransactionIdDeleteRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSubscriptionsTransactionIdDelete200Response(transactionId));

        // Act
        var actual = await _fundConnextService.CancelSwitchingOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672312180000831",
            OrderSide = OrderSide.Buy,
            Force = true
        }, It.IsAny<CancellationToken>());

        // Assert
        Assert.IsType<CancelFundOrderResponse>(actual);
        Assert.Equal(transactionId, actual.TransactionId);
    }

    [Fact]
    public async Task Should_Delete_RedemptionFundOrder_When_ApiRedemptionsTransactionIdDeleteAsync_Executed()
    {
        // Arrange
        const string transactionId = "1672312180000831";
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.Setup(q => q.ApiRedemptionsTransactionIdDeleteAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ApiSubscriptionsTransactionIdDeleteRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiSubscriptionsTransactionIdDelete200Response(transactionId));

        // Act
        var actual = await _fundConnextService.CancelRedemptionOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672312180000831",
            OrderSide = OrderSide.Buy,
            Force = true
        }, It.IsAny<CancellationToken>());

        // Assert
        Assert.IsType<CancelFundOrderResponse>(actual);
        Assert.Equal(transactionId, actual.TransactionId);
    }

    [Fact]
    public async Task Should_Return_FundOrders_When_GetAccountFundOrdersAsync_And_NextItNotNull()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.SetupSequence(q => q.ApiAccountFundOrdersV2GetAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountFundOrdersV2Get200Response(1, 1, true, new List<FundOrder>() { FakeFundOrder() }))
            .ReturnsAsync(new ApiAccountFundOrdersV2Get200Response(2, 1, true, new List<FundOrder>() { FakeFundOrder() }))
            .ReturnsAsync(new ApiAccountFundOrdersV2Get200Response(3, 1, false, new List<FundOrder>() { FakeFundOrder() }));

        // Act
        var actual = await _fundConnextService.GetAccountFundOrdersAsync("accountNo", new DateOnly(), new DateOnly());

        // Assert
        Assert.IsType<List<Application.Models.FundOrder>>(actual);
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async Task Should_Return_EmptyFundOrders_When_GetAccountFundOrdersAsync_And_Error()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.SetupSequence(q => q.ApiAccountFundOrdersV2GetAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        var actual = await _fundConnextService.GetAccountFundOrdersAsync("accountNo", new DateOnly(), new DateOnly());

        // Assert
        Assert.IsType<List<Application.Models.FundOrder>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Return_FundAssetResponse_When_GetAccountBalanceAsync()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.SetupSequence(q => q.ApiAccountBalancesGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountBalancesGet200Response(new List<FundAsset>() { new(navDate: "20231023") }));

        // Act
        var actual = await _fundConnextService.GetAccountBalanceAsync("accountNo");

        // Assert
        Assert.IsType<List<FundAssetResponse>>(actual);
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task Should_Return_CustomerAccountUnitHoldersResponse_When_GetCustomerAccountsync()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.SetupSequence(q =>
                q.ApiCustomerAccountGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new IndividualInvestorV5Response(accounts: new List<AccountV5Response> { FakeAccountV5Response() }));

        // Act
        var actual = await _fundConnextService.GetCustomerAccountByAccountNoAsync("0800547-M");

        // Assert
        Assert.IsType<CustomerAccountDetail>(actual);
        Assert.NotEmpty(actual.CustomerAccountUnitHolders);
    }

    [Fact]
    public async Task Should_Return_EmptyFundAssetResponse_When_GetAccountBalanceAsync_And_Error()
    {
        // Arrange
        _distributedCache.Setup(q => q.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some token"u8.ToArray());
        _fundConnextApi.SetupSequence(q => q.ApiAccountBalancesGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        var actual = await _fundConnextService.GetAccountBalanceAsync("accountNo");

        // Assert
        Assert.IsType<List<FundAssetResponse>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Return_EmptyRawFundOrder()
    {
        // Arrange
        _fundConnextApi.Setup(q => q.ApiFundOrdersV2GetAsync(It.IsAny<string>(),
                It.IsAny<string?>(),
                null,
                null,
                null,
                null,
                null,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountFundOrdersV2Get200Response());

        // Act
        var actual = await _fundConnextService.GetRawFundOrdersAsync(new DateOnly(2024, 7, 1), It.IsAny<CancellationToken>());

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.Empty(actual);
    }

    private static FundOrder FakeFundOrder()
    {
        return new FundOrder("1672312180000831",
            "374113",
            "SUB",
            "77112531",
            "9390103768",
            "KFGTECH-A",
            "UNIT",
            100,
            500,
            null,
            FundOrder.StatusEnum.EXPIRED,
            "20231218151257",
            "20231218",
            "20231218",
            null,
            100,
            100,
            100,
            "20231218",
            100,
            "20231218141257",
            "ATS_SA",
            "025",
            "0019148331",
            "MOB",
            "028773",
            "00",
            "N",
            "",
            "",
            null,
            null,
            "20231218",
            "",
            "OMN");
    }

    private static AccountV5Response FakeAccountV5Response()
    {
        var unitHolder =
            new UnitholderResponse
            {
                AmcCode = "AMCCode",
                UnitholderId = "DMC43533",
                AccountId = "0800547-M",
                UnitholderType = "OMN",
                Status = UnitholderResponse.StatusEnum.ACTIVE
            };
        return new AccountV5Response
        {
            AccountId = "0800547-M",
            Unitholders = new List<UnitholderResponse> { unitHolder }
        };
    }
}
