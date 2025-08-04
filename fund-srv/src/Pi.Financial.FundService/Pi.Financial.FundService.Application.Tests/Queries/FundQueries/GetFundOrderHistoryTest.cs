using Moq;
using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Queries.FundQueries;

public class GetFundOrderHistoryTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Application.Queries.FundQueries _fundQueries;
    private readonly Mock<IBankInfoRepository> _bankInfoRepository;
    private readonly Mock<IFundOrderRepository> _fundOrderRepository;

    public GetFundOrderHistoryTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _bankInfoRepository = new Mock<IBankInfoRepository>();
        Mock<IUserService> userService = new();
        Mock<IOnboardService> onboardService = new();
        _fundOrderRepository = new Mock<IFundOrderRepository>();

        _fundQueries = new Application.Queries.FundQueries(_fundConnextService.Object,
            onboardService.Object,
            _marketService.Object,
            _bankInfoRepository.Object,
            Mock.Of<IItBackofficeService>(),
            userService.Object,
            _fundOrderRepository.Object);
    }

    [Theory]
    [InlineData("FOSUB2024027")]
    public async Task Should_Return_FundOrders_History_With_FundInfo_And_BankInfo(string orderNo)
    {
        // Arrange
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), [FundOrderStatus.Submitted]);
        var fundOrders = new List<FundOrder>() { FakeFundOrder("TNEXTGEN-A", "033") };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        var bankInfo = new BankInfo(Guid.NewGuid(), "bank name", "BYY", "033", "some url");

        _fundConnextService
            .Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });
        _bankInfoRepository
            .Setup(q => q.GetByBankCodesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BankInfo>() { bankInfo });

        // Act
        var actual = await _fundQueries.GetFundOrdersByOrderNoAsync([orderNo], filters, CancellationToken.None);

        // Assert
        Assert.IsType<List<FundOrder>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundOrders.Count, actual.Count);
        Assert.NotNull(actual.First().FundInfo);
        Assert.NotNull(actual.First().BankInfo);
    }

    [Theory]
    [InlineData("FOSUB2024027")]
    public async Task Should_Return_FundOrderFromDatabase(string orderNo)
    {
        // Arrange
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), [FundOrderStatus.Submitted]);

        _fundConnextService
            .Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _fundOrderRepository
            .Setup(x => x.GetByOrderNoAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new FundOrderState(Guid.NewGuid(), "", "", Channel.MOB) { OrderNo = orderNo }]);

        // Act
        var actual = await _fundQueries.GetFundOrdersByOrderNoAsync([orderNo], filters, CancellationToken.None);

        // Assert
        Assert.Single(actual);
    }

    [Theory]
    [InlineData("INCORRECT_ORDER_NO")]
    public async Task Should_Return_EmptyRawFundOrder(string orderNo)
    {
        // Arrange
        var filters = new FundOrderFilters(new DateOnly(), new DateOnly(), [FundOrderStatus.Submitted]);

        _fundConnextService
            .Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _fundOrderRepository
            .Setup(x => x.GetByOrderNoAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var actual = await _fundQueries.GetFundOrdersByOrderNoAsync([orderNo], filters, CancellationToken.None);

        // Assert
        Assert.Empty(actual);
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
