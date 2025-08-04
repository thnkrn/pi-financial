using Moq;
using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Queries.FundQueries;

public class GetAccountSummariesAsyncTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IItBackofficeService> _itBackofficeService;
    private readonly Application.Queries.FundQueries _fundQueries;

    public GetAccountSummariesAsyncTest()
    {
        var bankInfoRepository = new Mock<IBankInfoRepository>();
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _itBackofficeService = new Mock<IItBackofficeService>();

        _fundQueries = new Application.Queries.FundQueries(
            _fundConnextService.Object,
            _onboardService.Object,
            _marketService.Object,
            bankInfoRepository.Object,
            _itBackofficeService.Object,
            _userService.Object,
            Mock.Of<IFundOrderRepository>());
    }

    [Fact]
    public async Task Should_ReturnExpected_When_Success()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var custCodes = new[] { "0900432", "0800081" };
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse("TNEXTGEN-A") };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(custCodes);
        _fundConnextService.Setup(q => q.CheckAccountExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _fundConnextService
            .SetupSequence(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets)
            .ReturnsAsync([])
            .ReturnsAsync(fundAssets)
            .ReturnsAsync([]);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });

        // Act
        var actual = await _fundQueries.GetAccountSummariesAsync(userId, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountSummary>>(actual);
        Assert.Equal(custCodes.Length * 2, actual.Count);
        Assert.Equal(["0900432-M", "0900432-1", "0800081-M", "0800081-1"], actual.Select(q => q.TradingAccountNo));
        Assert.Equal([fundAssets.Count, 0, fundAssets.Count, 0], actual.Select(q => q.Assets.Count()));
    }

    [Fact]
    public async Task Should_ReturnExpected_When_OnlyOneTradingAccountRegistered()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var custCodes = new[] { "0900432" };
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse("TNEXTGEN-A") };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(custCodes);
        _fundConnextService.SetupSequence(q => q.CheckAccountExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });

        // Act
        var actual = await _fundQueries.GetAccountSummariesAsync(userId, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountSummary>>(actual);
        Assert.Equal(custCodes.Length, actual.Count);
        Assert.Equal(["0900432-M"], actual.Select(q => q.TradingAccountNo));
        Assert.True(actual.All(q => q.Assets.Count() == fundAssets.Count));
    }

    [Fact]
    public async Task Should_ReturnExpected_When_GetAccountBalanceAsyncError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var custCodes = new[] { "0900432" };
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(custCodes);
        _fundConnextService.Setup(q => q.CheckAccountExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await _fundQueries.GetAccountSummariesAsync(userId, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountSummary>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_ReturnExpected_When_CheckAccountExistError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var custCodes = new[] { "0900432" };
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(custCodes);
        _fundConnextService.Setup(q => q.CheckAccountExist(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await _fundQueries.GetAccountSummariesAsync(userId, CancellationToken.None);

        // Assert
        Assert.IsType<List<AccountSummary>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Error_When_CustCodesWasEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var action = async () => await _fundQueries.GetAccountSummariesAsync(userId, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    private static FundAssetResponse FakeFundAssetResponse(string? fundCode = null)
    {
        return new()
        {
            UnitholderId = "123456789",
            FundCode = fundCode ?? "F-FUND-123",
            Unit = (decimal)50.123,
            Amount = (decimal)500.50,
            RemainUnit = (decimal)25.0,
            RemainAmount = (decimal)250.25,
            PendingAmount = 0,
            PendingUnit = 0,
            AvgCost = (decimal)10.0,
            Nav = (decimal)12.345,
            NavDate = new DateOnly()
        };
    }
}
