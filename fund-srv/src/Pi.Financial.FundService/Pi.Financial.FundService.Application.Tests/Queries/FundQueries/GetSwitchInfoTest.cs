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

public class GetSwitchInfoTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Application.Queries.FundQueries _fundQueries;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IItBackofficeService> _itBackofficeService;

    public GetSwitchInfoTest()
    {
        var bankInfoRepository = new Mock<IBankInfoRepository>();
        _fundConnextService = new Mock<IFundConnextService>();
        _marketService = new Mock<IMarketService>();
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _itBackofficeService = new Mock<IItBackofficeService>();

        _fundQueries = new Application.Queries.FundQueries(_fundConnextService.Object,
            _onboardService.Object,
            _marketService.Object,
            bankInfoRepository.Object,
            Mock.Of<IItBackofficeService>(),
            _userService.Object,
            Mock.Of<IFundOrderRepository>());
    }

    [Fact]
    public async Task
        Should_ReturnMinUnitEqualMinSellUnit_When_RemainUnitLowerThanMinSellUnit()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 0;
        counterFundInfo.FirstMinBuyAmount = 0;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 40,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainUnit = 40
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundInfo.MinSellUnit, actual.MinSwitchUnit);
    }

    [Fact]
    public async Task
        Should_ReturnMinUnitEqualRemainUnit_When_RemainUnitGreaterThanMinSellUnit_And_UnitAfterMinSellLowerThanMinBalanceUnit()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("kgftech-a", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 0;
        counterFundInfo.FirstMinBuyAmount = 0;
        var fundAsset = new FundAsset(fundInfo.FundCode, "0800468", "7799113-M", "086220300014")
        {
            Unit = 60,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainUnit = 60
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundAsset.RemainUnit, actual.MinSwitchUnit);
    }

    [Fact]
    public async Task
        Should_ReturnMinUnitEqualMinSellUnit_When_RemainUnitGreaterThanMinSellUnit_And_UnitAfterMinSellGreaterThanMinBalanceUnit()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 0;
        counterFundInfo.FirstMinBuyAmount = 0;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainUnit = 110
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundInfo.MinSellUnit, actual.MinSwitchUnit);
    }

    [Fact]
    public async Task
        Should_ReturnMinAmountEqualMinSellAmount_When_RemainAmountLowerThanMinSellAmount()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 40,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 40
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundInfo.MinSellAmount, actual.MinSwitchAmount);
    }

    [Fact]
    public async Task
        Should_ReturnMinAmountEqualRemainAmount_When_RemainAmountGreaterThanMinSellAmount_And_AmountAfterMinSellLowerThanMinBalanceAmount()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellAmount = 50;
        fundInfo.MinBalanceAmount = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 0;
        counterFundInfo.FirstMinBuyAmount = 0;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 60,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 60
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundAsset.RemainAmount, actual.MinSwitchAmount);
    }

    [Fact]
    public async Task
        Should_ReturnMinAmountEqualMinSellAmount_When_RemainAmountGreaterThanMinSellAmount_And_AmountAfterMinSellGreaterThanMinBalanceAmount()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellAmount = 50;
        fundInfo.MinBalanceAmount = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 0;
        counterFundInfo.FirstMinBuyAmount = 0;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 110
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(fundInfo.MinSellAmount, actual.MinSwitchAmount);
    }

    [Fact]
    public async Task
        Should_ReturnMinAmountEqualCounterFirstMinBuyAmount_When_NextMinBuyAmountOfCounterFund_GreaterThan_SourceFundMinAmount()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellAmount = 50;
        fundInfo.MinBalanceAmount = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.FirstMinBuyAmount = 60;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 110
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(counterFundInfo.FirstMinBuyAmount, actual.MinSwitchAmount);
    }

    [Fact]
    public async Task
        Should_ReturnMinAmountEqualCounterNextMinBuyAmount_When_NextMinBuyAmountOfCounterFund_GreaterThan_SourceFundMinAmount_And_CounterFundExistInAccountBalance()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.FirstMinBuyAmount = 60;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 110
        };
        var counterFundAsset = new FundAsset(counterFundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        MockValidation(new List<FundAsset>() { fundAsset, counterFundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(counterFundInfo.NextMinBuyAmount, actual.MinSwitchAmount);
    }

    [Fact]
    public async Task
        Should_ReturnMinUnitEqualCounterMinBuyUnit_When_CounterMinBuyUnit_GreaterThan_SourceFundMinUnit_And_CounterAssetIsNotNull()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellUnit = 50;
        fundInfo.MinBalanceUnit = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 10;
        counterFundInfo.FirstMinBuyAmount = 60;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 110
        };
        var counterFundAsset = new FundAsset(counterFundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        var expectedMinUnit = counterFundInfo.NextMinBuyAmount / counterFundAsset.MarketPrice;
        MockValidation(new List<FundAsset>() { fundAsset, counterFundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(decimal.Round(expectedMinUnit, 4), decimal.Round(actual.MinSwitchUnit, 4));
    }

    [Fact]
    public async Task
        Should_ReturnMinUnitEqualCounterMinBuyUnit_When_CounterMinBuyUnit_GreaterThan_SourceFundMinUnit_And_CounterAssetIsNull()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundInfo = FakeFactory.NewFundInfo("KFGTECH-A", "KSAM");
        fundInfo.MinSellAmount = 50;
        fundInfo.MinSellUnit = 0;
        fundInfo.MinBalanceAmount = 50;
        var counterFundInfo = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        counterFundInfo.Nav = 10;
        counterFundInfo.NextMinBuyAmount = 60;
        counterFundInfo.FirstMinBuyAmount = 100;
        var fundAsset = new FundAsset(fundInfo.FundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 110,
            AsOfDate = new DateOnly(),
            MarketPrice = 5.30m,
            AvgCostPrice = 5.40m,
            RemainAmount = 110
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });
        var expectedMinUnit = counterFundInfo.FirstMinBuyAmount / counterFundInfo.Nav;
        MockValidation(new List<FundAsset>() { fundAsset }, new[] { fundInfo, counterFundInfo });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundInfo.FundCode, counterFundInfo.FundCode);

        // Assert
        Assert.Equal(decimal.Round((decimal)expectedMinUnit, 4), decimal.Round(actual.MinSwitchUnit, 4));
    }

    [Fact]
    public async Task Should_MinAmountAndMinUnitEqualZero_When_FundAssetNotFound()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundCode = "KF-OIL";
        _onboardService.Setup(q =>
                q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "771133",
                SaleLicense = "0036",

            });
        _fundConnextService.Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse>() { new()
                {
                    UnitholderId = "123456789",
                    FundCode = "MISMATCHED",
                    Unit = (decimal)10.123,
                    Amount = (decimal)400.50,
                    RemainUnit = (decimal)15.0,
                    RemainAmount = (decimal)200.25,
                    PendingAmount = 0,
                    PendingUnit = 0,
                    AvgCost = (decimal)10.0,
                    Nav = (decimal)11.345,
                    NavDate = new DateOnly()
                }
            });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var actual = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundCode, "KF-CHINA");

        // Assert
        Assert.Equal(0, actual.MinSwitchAmount);
        Assert.Equal(0, actual.MinSwitchUnit);
    }

    [Fact]
    public async Task Should_FOE102Failed_When_TradingAccountNoInvalid()
    {
        // Arrange
        var tradingAccountNo = "99999";

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, "KF-OIL", "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Fact]
    public async Task Should_FOE102Failed_When_TradingAccountNotFound()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, "KF-OIL", "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Fact]
    public async Task Should_Errors_When_UserCustCodeMisMatched()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "111333", "333111" });

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, "KF-OIL", "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE102, exception.Code);
    }

    [Fact]
    public async Task Should_FOE109Failed_When_FundAssetNAVIsZero()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundCode = "KF-OIL";
        var fund = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        fund.Nav = 0;
        _onboardService.Setup(q =>
                q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "771133",
                SaleLicense = "0036"
            });
        _fundConnextService.Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse>() { new()
                {
                    UnitholderId = "123456789",
                    FundCode = fundCode,
                    Unit = (decimal)10.123,
                    Amount = (decimal)400.50,
                    RemainUnit = (decimal)15.0,
                    RemainAmount = (decimal)200.25,
                    PendingAmount = 0,
                    PendingUnit = 0,
                    AvgCost = (decimal)10.0,
                    Nav = 0m,
                    NavDate = new DateOnly()
                }
            });
        _marketService.Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { fund });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundCode, "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE109, exception.Code);
    }

    [Fact]
    public async Task Should_FOE101Failed_When_FundNotFound()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fundCode = "KF-OIL";
        var fund = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        _onboardService.Setup(q =>
                q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "771133",
                SaleLicense = "0036"
            });
        _fundConnextService.Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse>() { new()
                {
                    UnitholderId = "123456789",
                    FundCode = fundCode,
                    Unit = (decimal)10.123,
                    Amount = (decimal)400.50,
                    RemainUnit = (decimal)15.0,
                    RemainAmount = (decimal)200.25,
                    PendingAmount = 0,
                    PendingUnit = 0,
                    AvgCost = (decimal)10.0,
                    Nav = (decimal)11.345,
                    NavDate = new DateOnly()
                }
            });
        _marketService.Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { fund });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fundCode, "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE101, exception.Code);
    }

    [Fact]
    public async Task Should_FOE101Failed_When_CounterFundNotFound()
    {
        // Arrange
        var tradingAccountNo = "0800468-M";
        var fund = FakeFactory.NewFundInfo("KF-BIC", "KSAM");
        _onboardService.Setup(q =>
                q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "771133",
                SaleLicense = "0036"
            });
        _fundConnextService.Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundAssetResponse>() { new()
                {
                    UnitholderId = "123456789",
                    FundCode = fund.FundCode,
                    Unit = (decimal)10.123,
                    Amount = (decimal)400.50,
                    RemainUnit = (decimal)15.0,
                    RemainAmount = (decimal)200.25,
                    PendingAmount = 0,
                    PendingUnit = 0,
                    AvgCost = (decimal)10.0,
                    Nav = (decimal)11.345,
                    NavDate = new DateOnly()
                }
            });
        _marketService.Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { fund });
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccountNo[..^2] });

        // Act
        var action = async () => await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(Guid.NewGuid(), tradingAccountNo, fund.FundCode, "KF-CHINA");

        // Assert
        var exception = await Assert.ThrowsAsync<FundOrderException>(action);
        Assert.Equal(FundOrderErrorCode.FOE101, exception.Code);
    }

    private void MockValidation(List<FundAsset> assets, FundInfo[] fundInfos)
    {
        _onboardService.Setup(q =>
                q.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TradingAccountInfo
            {
                Id = Guid.NewGuid(),
                AccountNo = "771133",
                SaleLicense = "0036"
            });

        _fundConnextService.Setup(q => q.GetAccountBalanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assets.Select(q =>
            {
                return new FundAssetResponse
                {
                    UnitholderId = q.UnitHolderId,
                    FundCode = q.FundCode,
                    Unit = q.Unit,
                    Amount = q.MarketValue,
                    RemainUnit = q.RemainUnit,
                    RemainAmount = q.RemainAmount,
                    PendingAmount = q.PendingAmount,
                    PendingUnit = q.PendingUnit,
                    AvgCost = q.AvgCostPrice,
                    Nav = q.MarketPrice,
                    NavDate = q.AsOfDate
                };
            }).ToList());
        _marketService.Setup(q => q.GetFundInfosAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundInfos);
    }
}
