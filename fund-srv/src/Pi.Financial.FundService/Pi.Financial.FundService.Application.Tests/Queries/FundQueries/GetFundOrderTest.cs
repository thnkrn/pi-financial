using Moq;
using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Queries.FundQueries;

public class GetFundOrderTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Application.Queries.FundQueries _fundQueries;
    private readonly Mock<IBankInfoRepository> _bankInfoRepository;
    private readonly Mock<IUserService> _userService;

    public GetFundOrderTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _onboardService = new Mock<IOnboardService>();
        _bankInfoRepository = new Mock<IBankInfoRepository>();
        _userService = new Mock<IUserService>();

        _fundQueries = new Application.Queries.FundQueries(_fundConnextService.Object,
            _onboardService.Object,
            _marketService.Object,
            _bankInfoRepository.Object,
            Mock.Of<IItBackofficeService>(),
            _userService.Object,
            Mock.Of<IFundOrderRepository>());
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_FundOrders_With_FundInfo_And_BankInfo(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Submitted });
        var fundOrders = new List<FundOrder>() { FakeFundOrder("TNEXTGEN-A", "033") };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        var bankInfo = new BankInfo(Guid.NewGuid(), "bank name", "BYY", "033", "some url");
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });
        _bankInfoRepository
            .Setup(q => q.GetByBankCodesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BankInfo>() { bankInfo });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundOrders.Count, actual.Count);
        Assert.NotNull(actual.First().FundInfo);
        Assert.NotNull(actual.First().BankInfo);
    }

    [Theory]
    [InlineData(FundOrderStatus.Cancelled)]
    [InlineData(FundOrderStatus.Rejected)]
    [InlineData(FundOrderStatus.Allotted)]
    public async Task Should_Return_ExpectedOrderTypeFundOrders_When_StatusFiltersNotNull(FundOrderStatus status)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tradingAccountNo = "7711431-M";
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { status });
        var fundOrders = new List<FundOrder>() { FakeFundOrder("TNEXTGEN-A", "033", status) };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundOrders.Count, actual.Count);
        Assert.Equal(status, actual.First().Status);
    }

    [Theory]
    [InlineData(FundOrderType.Subscription)]
    [InlineData(FundOrderType.SwitchOut)]
    [InlineData(FundOrderType.CrossSwitchIn)]
    public async Task Should_Return_ExpectedOrderTypeFundOrders_When_OrderTypeFiltersNotNull(FundOrderType? orderType)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tradingAccountNo = "7711431-M";
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Submitted }, orderType);
        var fundOrders = new List<FundOrder>() { FakeFundOrder("TNEXTGEN-A", "033", null, orderType) };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundOrders.Count, actual.Count);
        Assert.Equal(orderType, actual.First().OrderType);
    }

    [Theory]
    [InlineData(FundOrderStatus.Cancelled, FundOrderType.Redemption)]
    [InlineData(FundOrderStatus.Rejected, FundOrderType.Redemption)]
    [InlineData(FundOrderStatus.Allotted, FundOrderType.Subscription)]
    public async Task Should_Return_EmptyFundOrders_When_OrderTypeFilterIsNotNull_And_RecordMismatch(FundOrderStatus orderStatus, FundOrderType fundOrderType)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tradingAccountNo = "7711431-M";
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { orderStatus }, fundOrderType);
        var fundOrders = new List<FundOrder>() { FakeFundOrder("TNEXTGEN-A", "033", FundOrderStatus.Rejected, FundOrderType.Subscription) };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.Empty(actual);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_FundOrders_Without_Info_When_FundInfoIsEmpty(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Submitted });
        var fundOrders = new List<FundOrder>() { FakeFundOrder() };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>());
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundOrders.Count, actual.Count);
        Assert.Null(actual.First().FundInfo);
        Assert.Null(actual.First().BankInfo);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_EmptyFundOrders_When_EmptyFundOrders(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Allotted });
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountFundOrdersAsync(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>());
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.Empty(actual);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    [InlineData("7711431-2")]
    [InlineData("7711444-S")]
    [InlineData("7711440-10")]
    public async Task Should_Error_When_TradingAccountNotFoundOrInvalid(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Allotted });

        // Act
        var action = async () => await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Fact]
    public async Task Should_Errors_When_UserCustCodeMisMatched()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tradingAccountNo = "7711440-M";
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), new[] { FundOrderStatus.Allotted });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "111333", "333111" });

        // Act
        var action = async () => await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, filters, cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Fact]
    public async Task Should_Return_EmptyRawFundOrder()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _fundConnextService
            .Setup(x => x.GetRawFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var results = await _fundQueries.GetRawOrdersAsync(new DateOnly(), cancellationToken);

        // Assert
        Assert.Empty(results);
    }


    private static FundOrder FakeFundOrder(string? fundCode = null, string? bankCode = null, FundOrderStatus? fundStatus = null, FundOrderType? fundOrderType = null)
    {
        return new()
        {
            FundCode = fundCode ?? "F-FUND-123",
            Unit = (decimal)50.123,
            Amount = (decimal)500.50,
            PaymentMethod = "TRN",
            BankAccount = "bank account",
            Edd = null,
            SwitchIn = null,
            SwitchTo = null,
            BankCode = bankCode ?? "034",
            BrokerOrderId = "123123414",
            AccountId = "1321321",
            OrderType = fundOrderType ?? FundOrderType.Subscription,
            Status = fundStatus ?? FundOrderStatus.Submitted,
            TransactionLastUpdated = DateTime.Now,
            EffectiveDate = new DateOnly(),
            TransactionDateTime = DateTime.Now,
            OrderNo = "FOSUB2024027",
            UnitHolderId = "77991131",
            AllottedAmount = (decimal)50.123,
            AllottedUnit = (decimal)50.123,
            AllottedNav = (decimal)50.123,
            SellAllUnit = true,
            CounterFundCode = "ABC-D",
            PaymentType = PaymentType.AtsAmc,
            AccountType = FundAccountType.OMN,
            RedemptionType = RedemptionType.Amount,
            OrderSide = OrderSide.Buy,
            AmcOrderId = "7787484784",
            NavDate = new DateOnly(),
            Channel = Channel.MOB,
            AllottedDate = new DateOnly(),
            SettlementDate = new DateOnly(),
            RejectReason = null,
            SettlementBankAccount = null,
            SettlementBankCode = null,
            SaleLicense = "hdk",
        };
    }
}
