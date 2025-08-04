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

public class GetFundAssetTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;
    private readonly Application.Queries.FundQueries _fundQueries;
    private readonly Mock<IItBackofficeService> _itBackofficeService;

    public GetFundAssetTest()
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

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_FundAssets_With_Info(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse("TNEXTGEN-A") };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTgen-A", taxType: "SSF");
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

        // Assert
        Assert.IsType<List<FundAsset>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundAssets.Count, actual.Count);
        Assert.NotNull(actual.First().Info);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_FundAssets_When_FallbackAssets_Is_Not_Null(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse("TNEXTGEN-A") with
        {
            Unit = 0,
            Amount = 0,
            RemainUnit = 0,
            RemainAmount = 0,
        }, FakeFundAssetResponse("KF-OIL") };
        var fallbackAssets = new List<BoFundAssetResponse>() { new()
            {
                AmcCode = "ASSETFUND",
                AccountId = tradingAccountNo,
                UnitholderId = "123456789",
                FundCode = "TNEXTGEN-A",
                Unit = (decimal)50.123,
                Amount = (decimal)500.50,
                RemainUnit = (decimal)25.0,
                RemainAmount = (decimal)250.25,
                PendingAmount = 0,
                PendingUnit = 0,
                AvgCost = (decimal)10.0,
                Nav = (decimal)12.345,
                NavDate = new DateOnly()
            }
        };
        var fundInfo = FakeFactory.NewFundInfo("TNEXTGEN-A", taxType: "SSF");
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>() { fundInfo });
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        _itBackofficeService.Setup(q =>
            q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fallbackAssets);

        // Act
        var actual = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

        // Assert
        Assert.IsType<List<FundAsset>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundAssets.Count, actual.Count);
        Assert.NotNull(actual.First().Info);
        Assert.Equal(actual.Select(q => q.FundCode), ["TNEXTGEN-A", "KF-OIL"]);
        Assert.Equal(actual.Select(q => q.Unit), [50.123m, 50.123m]);
        Assert.Equal(actual.Select(q => q.RemainUnit), [25.0m, 25.0m]);
        Assert.Equal(actual.Select(q => q.RemainAmount), [250.25m, 250.25m]);
        Assert.Equal(actual.Select(q => q.PendingUnit), [0m, 0m]);
        Assert.Equal(actual.Select(q => q.PendingAmount), [0m, 0m]);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_FundAssets_Without_Info_When_FundInfoIsEmpty(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse() };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>());
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

        // Assert
        Assert.IsType<List<FundAsset>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundAssets.Count, actual.Count);
        Assert.Null(actual.First().Info);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    public async Task Should_Return_EmptyFundAssets_When_EmptyFundAssets(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse>());
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

        // Assert
        Assert.IsType<List<FundAsset>>(actual);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task Should_Return_ExpectedFundAssets_When_TradingAccountNoIsNull()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var fundAssets = new List<FundAssetResponse>() { FakeFundAssetResponse() };
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });
        _fundConnextService
            .Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundAssets);
        _marketService
            .Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundInfo>());
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "771199", "771188" });

        // Act
        var actual = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), null, cancellationToken);

        // Assert
        Assert.IsType<List<FundAsset>>(actual);
        Assert.NotEmpty(actual);
        Assert.Equal(fundAssets.Count * 4, actual.Count);
    }

    [Fact]
    public async Task Should_Errors_When_CustCodeNotFound()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var action = async () => await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), "7711444-1", cancellationToken);

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Theory]
    [InlineData("7711431-M")]
    [InlineData("7711444-1")]
    [InlineData("7711440-M")]
    [InlineData("7711431-2")]
    [InlineData("7711444-S")]
    [InlineData("7711440-10")]
    public async Task Should_Errors_When_TradingAccountNotFoundOrInvalid(string tradingAccountNo)
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "111333", "333111" });

        // Act
        var action = async () => await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

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
        _userService.Setup(q => q.GetFundCustomerCodesByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "111333", "333111" });
        _onboardService.Setup(q => q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "77110099",
                SaleLicense = "7632"
            });

        // Act
        var action = async () => await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, cancellationToken);

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
