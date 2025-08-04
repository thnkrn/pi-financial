using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Tests.Models;

public class EquityAssetTest
{
    [Theory]
    [InlineData(5.10, 100, 510.00)]
    [InlineData(3.256, 100, 325.6)]
    public void Should_Return_ExpectedMarketValue_When_Init_EquityAsset(decimal marketPrice, int volume,
        decimal expected)
    {
        // Act
        var asset = new EquityAsset
        {
            AccountNo = "08010776",
            Symbol = "EA",
            Nvdr = false,
            AverageCostPrice = 6.546m,
            AvailableVolume = volume,
            SellableVolume = volume,
            Amount = 654.6m,
            IsNew = false,
            MarketPrice = marketPrice,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            StockType = StockType.Normal,
            CorporateActions = new List<CorporateAction>()
        };

        // Assert
        Assert.Equal(expected, asset.MarketValue);
    }

    [Theory]
    [InlineData(510.00, 100, StockType.Normal, 510.00)]
    [InlineData(325.6, 100, StockType.Normal, 325.6)]
    [InlineData(325.6, 100, StockType.Borrow, 0)]
    public void Should_Return_ExpectedCostValue_When_Init_EquityAsset(decimal amount, int volume, StockType stockType,
        decimal expected)
    {
        // Act
        var asset = new EquityAsset
        {
            AccountNo = "08010776",
            Symbol = "EA",
            Nvdr = false,
            AverageCostPrice = 6.546m,
            AvailableVolume = volume,
            SellableVolume = volume,
            Amount = amount,
            IsNew = false,
            MarketPrice = 5.10m,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            StockType = stockType,
            CorporateActions = new List<CorporateAction>()
        };

        // Assert
        Assert.Equal(expected, asset.CostValue);
    }

    [Theory]
    [InlineData(300.60, 6.1, 100, false, StockType.Normal, 309.4)]
    [InlineData(654.60, 5.1, 100, false, StockType.Normal, -144.60)]
    [InlineData(326100.78, 106.5, 3000, false, StockType.Short, 6600.78)]
    [InlineData(300.60, 6.1, 100, false, StockType.Borrow, 0)]
    [InlineData(654.60, 5.1, 100, false, StockType.Borrow, 0)]
    [InlineData(300.60, 6.1, 100, true, StockType.Borrow, 0)]
    [InlineData(654.60, 5.1, 100, true, StockType.Borrow, 0)]
    [InlineData(300.60, 6.1, 100, true, StockType.Normal, 0)]
    [InlineData(654.60, 5.1, 100, true, StockType.Normal, 0)]
    public void Should_Return_ExpectedUpnl_When_Init_EquityAsset(decimal amount, decimal marketPrice, int volume,
        bool isNew, StockType stockType, decimal expected)
    {
        // Act
        var asset = new EquityAsset
        {
            AccountNo = "08010776",
            Symbol = "EA",
            Nvdr = false,
            AverageCostPrice = 6.546m,
            AvailableVolume = volume,
            SellableVolume = volume,
            Amount = amount,
            IsNew = isNew,
            MarketPrice = marketPrice,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            StockType = stockType,
            CorporateActions = new List<CorporateAction>()
        };

        // Assert
        Assert.Equal(expected, asset.Upnl);
    }

    [Theory]
    [InlineData(300.60, 6.1, 100, false, StockType.Normal, 102.93)]
    [InlineData(654.60, 5.1, 100, false, StockType.Normal, -22.09)]
    [InlineData(326100.78, 106.5, 3000, false, StockType.Short, 2.02)]
    [InlineData(300.60, 6.1, 100, false, StockType.Borrow, 0)]
    [InlineData(654.60, 5.1, 100, false, StockType.Borrow, 0)]
    [InlineData(300.60, 6.1, 100, true, StockType.Borrow, 0)]
    [InlineData(654.60, 5.1, 100, true, StockType.Borrow, 0)]
    [InlineData(300.60, 6.1, 100, true, StockType.Normal, 0)]
    [InlineData(654.60, 5.1, 100, true, StockType.Normal, 0)]
    public void Should_Return_ExpectedUpnlPercentage_When_Init_EquityAsset(decimal amount, decimal marketPrice,
        int volume, bool isNew, StockType stockType, decimal expected)
    {
        // Act
        var asset = new EquityAsset
        {
            AccountNo = "08010776",
            Symbol = "EA",
            Nvdr = false,
            AverageCostPrice = 6.546m,
            AvailableVolume = volume,
            SellableVolume = volume,
            Amount = amount,
            IsNew = isNew,
            MarketPrice = marketPrice,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            StockType = stockType,
            CorporateActions = new List<CorporateAction>()
        };

        // Assert
        Assert.Equal(decimal.Round(expected, 2), decimal.Round(asset.UpnlPercentage, 2));
    }

    [Theory]
    [InlineData(StockType.Normal, 100, 0)]
    [InlineData(StockType.Borrow, 100, 0)]
    [InlineData(StockType.Short, 100, 0)]
    [InlineData(StockType.Unknow, 100, 0)]
    [InlineData(StockType.NewStockType8, 100, 0)]
    [InlineData(StockType.NewStockType82, 100, 0)]
    [InlineData(StockType.Lending, 100, 100)]
    public void Should_ReturnExpectedLendingVolume_When_Init_EquityAsset(StockType stockType, int volume, int expected)
    {
        // Act
        var asset = new EquityAsset
        {
            AccountNo = "08010776",
            Symbol = "EA",
            Nvdr = false,
            AverageCostPrice = 6.546m,
            AvailableVolume = volume,
            SellableVolume = volume,
            Amount = 654.6m,
            IsNew = false,
            MarketPrice = 5.10m,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            StockType = stockType,
            CorporateActions = new List<CorporateAction>()
        };

        // Assert
        Assert.Equal(expected, asset.LendingVolume);
    }
}
