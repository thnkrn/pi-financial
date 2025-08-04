using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Domain.Tests.AggregatesModel.FinancialAssetAggregate;

public class FundAssetTest
{
    [Theory]
    [InlineData(0, 18.6035, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(1, 18.6035, 18.6035)]
    [InlineData(1404.2583, 18.6035, 26124.1193)]
    [InlineData(31.5541, 17.1165, 540.0958)]
    public void Should_ReturnExpected_MarketValue(decimal unit, decimal marketPrice, decimal expected)
    {
        // Arrange
        var asset = new FundAsset("UNIT-A", "7799113", "7799113-M", "086220300014")
        {
            Unit = unit,
            AsOfDate = new DateOnly(),
            MarketPrice = marketPrice,
            AvgCostPrice = 1
        }; ;

        // Act
        var actual = asset.MarketValue;

        // Assert
        Assert.Equal(decimal.Round(expected, 4), decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData(0, 19.5714, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(1, 19.5714, 19.5714)]
    [InlineData(1404.2583, 19.5714, 27483.3009)]
    [InlineData(31.5541, 15.8458, 500.0000)]
    public void Should_ReturnExpected_CostValue(decimal unit, decimal avgCost, decimal expected)
    {
        // Arrange
        var asset = new FundAsset("UNIT-A", "7799113", "7799113-M", "086220300014")
        {
            Unit = unit,
            AsOfDate = new DateOnly(),
            MarketPrice = 1,
            AvgCostPrice = avgCost
        };

        // Act
        var actual = asset.CostValue;

        // Assert
        Assert.Equal(decimal.Round(expected, 4), decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData(0, 18.6035, 19.5714, 0)]
    [InlineData(1, 18.6035, 0, 18.6035)]
    [InlineData(1, 0, 19.5714, -19.5714)]
    [InlineData(1, 18.6035, 19.5714, -0.9679)]
    [InlineData(1404.2583, 18.6035, 19.5714, -1359.18162)]
    [InlineData(31.5541, 17.1165, 15.8458, 40.0958)]
    public void Should_ReturnExpected_UPNL(decimal unit, decimal marketPrice, decimal avgCost, decimal expected)
    {
        // Arrange
        var asset = new FundAsset("UNIT-A", "7799113", "7799113-M", "086220300014")
        {
            Unit = unit,
            AsOfDate = new DateOnly(),
            MarketPrice = marketPrice,
            AvgCostPrice = avgCost
        };

        // Act
        var actual = asset.UPNL;

        // Assert
        Assert.Equal(decimal.Round(expected, 4), decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData(0, 18.6035, 19.5714, 0)]
    [InlineData(1, 18.6035, 0, 0)]
    [InlineData(1, 0, 19.5714, -100)]
    [InlineData(10, 18.6035, 19.5714, -4.9455)]
    [InlineData(1404.2583, 18.6035, 19.5714, -4.9455)]
    [InlineData(31.5541, 17.1165, 15.8458, 8.0192)]
    public void Should_ReturnExpected_UPNLPercentage(decimal unit, decimal marketPrice, decimal avgCost, decimal expected)
    {
        // Arrange
        var asset = new FundAsset("UNIT-A", "7799113", "7799113-M", "086220300014")
        {
            Unit = unit,
            AsOfDate = new DateOnly(),
            MarketPrice = marketPrice,
            AvgCostPrice = avgCost
        };

        // Act
        var actual = asset.UPNLPercentage;

        // Assert
        Assert.Equal(decimal.Round(expected, 4), decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData("KF-OIL", "KF-OIL", true)]
    [InlineData("kf-oil", "KF-OIL", true)]
    [InlineData("KF-OIL", "KF-CHINA", false)]
    [InlineData("SCB-OIL", "KF-CHINA", false)]
    public void Should_ReturnExpected_When_SetInfo(string fundCode, string fundInfoCode, bool expected)
    {
        // Arrange
        var asset = new FundAsset(fundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 11,
            AsOfDate = new DateOnly(),
            MarketPrice = 11,
            AvgCostPrice = 11
        };
        var fundInfo = new FundInfo("Mock", fundInfoCode, "logo",
            "KSAM")
        {
            InstrumentCategory = "Fund",
            Nav = 0,
            FirstMinBuyAmount = 0,
            NextMinBuyAmount = 0,
            MinSellAmount = 0,
            MinSellUnit = 0,
            MinBalanceAmount = 0,
            MinBalanceUnit = 0,
            PiBuyCutOffTime = default,
            PiSellCutOffTime = default
        };

        // Act
        var actual = asset.SetInfo(fundInfo);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("KF-OIL", "KF-OIL")]
    [InlineData("kf-oil", "KF-OIL")]
    public void Should_SetFundInfo_When_SetInfo(string fundCode, string fundInfoCode)
    {
        // Arrange
        var asset = new FundAsset(fundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 11,
            AsOfDate = new DateOnly(),
            MarketPrice = 11,
            AvgCostPrice = 11
        };
        var fundInfo = new FundInfo("Mock", fundInfoCode, "logo",
            "KSAM")
        {
            InstrumentCategory = "Fund",
            Nav = 0,
            FirstMinBuyAmount = 0,
            NextMinBuyAmount = 0,
            MinSellAmount = 0,
            MinSellUnit = 0,
            MinBalanceAmount = 0,
            MinBalanceUnit = 0,
            PiBuyCutOffTime = default,
            PiSellCutOffTime = default
        };

        // Act
        asset.SetInfo(fundInfo);

        // Assert
        Assert.NotNull(asset.Info);
    }

    [Theory]
    [InlineData("KF-OIL", "KF-CHINA")]
    [InlineData("KF-CHINA", "KFCHINA")]
    public void Should_Not_SetFundInfo_When_SetInfo(string fundCode, string fundInfoCode)
    {
        // Arrange
        var asset = new FundAsset(fundCode, "7799113", "7799113-M", "086220300014")
        {
            Unit = 11,
            AsOfDate = new DateOnly(),
            MarketPrice = 11,
            AvgCostPrice = 11
        };
        var fundInfo = new FundInfo("Mock", fundInfoCode, "logo",
            "KSAM")
        {
            Nav = 0,
            InstrumentCategory = "Fund",
            FirstMinBuyAmount = 0,
            NextMinBuyAmount = 0,
            MinSellAmount = 0,
            MinSellUnit = 0,
            MinBalanceAmount = 0,
            MinBalanceUnit = 0,
            PiBuyCutOffTime = default,
            PiSellCutOffTime = default
        };

        // Act
        asset.SetInfo(fundInfo);

        // Assert
        Assert.Null(asset.Info);
    }
}
