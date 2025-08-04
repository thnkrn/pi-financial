using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Tests.Models.AccountSummaries;

public class CashAccountSummaryTest
{
    [Fact]
    public void Should_Return_Expected_When_Init_And_Sbl_Disabled()
    {
        // Act
        var accountSummary = new CashAccountSummary
        {
            TradingAccountNo = "0801078-8",
            CustomerCode = "0801078",
            AccountNo = "08010788",
            AsOfDate = default,
            TraderId = "909",
            CreditLimit = 100000,
            BuyCredit = 200000,
            CashBalance = 300000,
            AccountType = TradingAccountType.Cash,
            SblEnabled = false,
            Ar = 10000,
            Ap = 23000,
            ArTrade = 1000,
            ApTrade = 2300,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0,
            Assets =
            [
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "EA",
                    Nvdr = false,
                    AverageCostPrice = 6.546m,
                    AvailableVolume = 100,
                    SellableVolume = 100,
                    Amount = 654.6m,
                    IsNew = false,
                    MarketPrice = 5.10m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Normal,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "MINT",
                    Nvdr = false,
                    AverageCostPrice = 30.75m,
                    AvailableVolume = 200,
                    SellableVolume = 200,
                    Amount = 6150m,
                    IsNew = false,
                    MarketPrice = 26.75m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Lending,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "EA",
                    Nvdr = false,
                    AverageCostPrice = 6.546m,
                    AvailableVolume = 100,
                    SellableVolume = 100,
                    Amount = 654.6m,
                    IsNew = true,
                    MarketPrice = 5.10m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Normal,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "MINT",
                    Nvdr = false,
                    AverageCostPrice = 30.75m,
                    AvailableVolume = 200,
                    SellableVolume = 200,
                    Amount = 6150m,
                    IsNew = false,
                    MarketPrice = 26.75m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Borrow,
                    CorporateActions = []
                },
            ],
            BackofficeAvailableBalance = new BackofficeAvailableBalance
            {
                TradingAccountNo = "0801078-8",
                AccountNo = "08010788",
                CashBalance = 200000,
                ArTrade = 300,
                ApTrade = 500,
                MarketValue = 10000,
                PostDateTime = default
            }
        };

        // Assert
        Assert.Equal(319060m, accountSummary.TotalValue);
        Assert.Equal(5860m, accountSummary.TotalMarketValue);
        Assert.Equal(6804.6m, accountSummary.TotalCost);
        Assert.Equal(-944.6m, accountSummary.TotalUpnl);
        Assert.Equal(-13.8818m, decimal.Round(accountSummary.TotalUpnlPercentage, 4));
        Assert.Equal(0, accountSummary.LongMarketValue);
        Assert.Equal(0, accountSummary.ShortMarketValue);
        Assert.Equal(0, accountSummary.LongCostValue);
        Assert.Equal(0, accountSummary.ShortCostValue);
    }

    [Fact]
    public void Should_Return_Expected_When_Init_And_Sbl_Enabled()
    {
        // Act
        var accountSummary = new CashAccountSummary
        {
            TradingAccountNo = "0801078-8",
            CustomerCode = "0801078",
            AccountNo = "08010788",
            AsOfDate = default,
            TraderId = "909",
            CreditLimit = 100000,
            BuyCredit = 200000,
            CashBalance = 300000,
            AccountType = TradingAccountType.Cash,
            SblEnabled = true,
            Ar = 10000,
            Ap = 23000,
            ArTrade = 1000,
            ApTrade = 2300,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0,
            Assets = DataTestEquityAssets(),
            BackofficeAvailableBalance = new BackofficeAvailableBalance
            {
                TradingAccountNo = "0801078-8",
                AccountNo = "08010788",
                CashBalance = 200000,
                ArTrade = 300,
                ApTrade = 500,
                MarketValue = 10000,
                PostDateTime = default
            }
        };

        // Assert
        Assert.Equal(365010.00m, accountSummary.TotalValue);
        Assert.Equal(51810.00m, accountSummary.TotalMarketValue);
        Assert.Equal(51022.28m, accountSummary.TotalCost);
        Assert.Equal(787.72m, accountSummary.TotalUpnl);
        Assert.Equal(1.54m, decimal.Round(accountSummary.TotalUpnlPercentage, 2));
        Assert.Equal(52670.00m, accountSummary.LongMarketValue);
        Assert.Equal(860.00m, accountSummary.ShortMarketValue);
        Assert.Equal(51826.95m, accountSummary.LongCostValue);
        Assert.Equal(804.67m, accountSummary.ShortCostValue);
    }

    [Fact]
    public void Should_Return_Expected_When_Init_And_Assets_Are_Empty()
    {
        // Act
        var accountSummary = new CashAccountSummary
        {
            TradingAccountNo = "0801078-8",
            CustomerCode = "0801078",
            AccountNo = "08010788",
            AsOfDate = default,
            TraderId = "909",
            CreditLimit = 100000,
            BuyCredit = 200000,
            CashBalance = 300000,
            AccountType = TradingAccountType.Cash,
            SblEnabled = true,
            Ar = 10000,
            Ap = 23000,
            ArTrade = 1000,
            ApTrade = 2300,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0,
            Assets = [],
            BackofficeAvailableBalance = new BackofficeAvailableBalance
            {
                TradingAccountNo = "0801078-8",
                AccountNo = "08010788",
                CashBalance = 200000,
                ArTrade = 300,
                ApTrade = 500,
                MarketValue = 10000,
                PostDateTime = default
            }
        };

        // Assert
        Assert.Equal(313200, accountSummary.TotalValue);
        Assert.Equal(0, accountSummary.TotalMarketValue);
        Assert.Equal(0, accountSummary.TotalCost);
        Assert.Equal(0, accountSummary.TotalUpnl);
        Assert.Equal(0, accountSummary.TotalUpnlPercentage);
        Assert.Equal(0, accountSummary.LongMarketValue);
        Assert.Equal(0, accountSummary.ShortMarketValue);
        Assert.Equal(0, accountSummary.LongCostValue);
        Assert.Equal(0, accountSummary.ShortCostValue);
    }

    [Fact]
    public void Should_Return_Expected_When_BackofficeIsNull()
    {
        // Act
        var accountSummary = new CashAccountSummary
        {
            TradingAccountNo = "0801078-8",
            CustomerCode = "0801078",
            AccountNo = "08010788",
            AsOfDate = default,
            TraderId = "909",
            CreditLimit = 100000,
            BuyCredit = 200000,
            CashBalance = 300000,
            AccountType = TradingAccountType.Cash,
            SblEnabled = false,
            Ar = 10000,
            Ap = 23000,
            ArTrade = 1000,
            ApTrade = 2300,
            TotalBuyMatch = 0,
            TotalBuyUnmatch = 0,
            Assets =
            [
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "EA",
                    Nvdr = false,
                    AverageCostPrice = 6.546m,
                    AvailableVolume = 100,
                    SellableVolume = 100,
                    Amount = 654.6m,
                    IsNew = false,
                    MarketPrice = 5.10m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Normal,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "MINT",
                    Nvdr = false,
                    AverageCostPrice = 30.75m,
                    AvailableVolume = 200,
                    SellableVolume = 200,
                    Amount = 6150m,
                    IsNew = false,
                    MarketPrice = 26.75m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Lending,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "EA",
                    Nvdr = false,
                    AverageCostPrice = 6.546m,
                    AvailableVolume = 100,
                    SellableVolume = 100,
                    Amount = 654.6m,
                    IsNew = true,
                    MarketPrice = 5.10m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Normal,
                    CorporateActions = []
                },
                new EquityAsset
                {
                    AccountNo = "08010788",
                    Symbol = "MINT",
                    Nvdr = false,
                    AverageCostPrice = 30.75m,
                    AvailableVolume = 200,
                    SellableVolume = 200,
                    Amount = 6150m,
                    IsNew = false,
                    MarketPrice = 26.75m,
                    Action = OrderAction.Buy,
                    Side = OrderSide.Buy,
                    StockType = StockType.Borrow,
                    CorporateActions = []
                },
            ],
            BackofficeAvailableBalance = null
        };

        // Assert
        Assert.Equal(318860m, accountSummary.TotalValue);
        Assert.Equal(5860m, accountSummary.TotalMarketValue);
        Assert.Equal(6804.6m, accountSummary.TotalCost);
        Assert.Equal(-944.6m, accountSummary.TotalUpnl);
        Assert.Equal(-13.8818m, decimal.Round(accountSummary.TotalUpnlPercentage, 4));
        Assert.Equal(0, accountSummary.LongMarketValue);
        Assert.Equal(0, accountSummary.ShortMarketValue);
        Assert.Equal(0, accountSummary.LongCostValue);
        Assert.Equal(0, accountSummary.ShortCostValue);
    }

    public static List<EquityAsset> DataTestEquityAssets()
    {
        return [
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "ADVANC",
                Nvdr = false,
                AverageCostPrice = 241.41m,
                AvailableVolume = 200m,
                SellableVolume = 200m,
                Amount = 48281.01m,
                IsNew = false,
                MarketPrice = 244.00m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Buy,
                Side = OrderSide.Buy,
                StockType = StockType.Normal
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "ADVANC",
                Nvdr = false,
                AverageCostPrice = 0.00m,
                AvailableVolume = 100m,
                SellableVolume = 100m,
                Amount = 0.00m,
                IsNew = false,
                MarketPrice = 244.00m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Borrow,
                Side = OrderSide.Buy,
                StockType = StockType.Borrow
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "EA",
                Nvdr = false,
                AverageCostPrice = 3.94m,
                AvailableVolume = 900m,
                SellableVolume = 900m,
                Amount = 3545.94m,
                IsNew = false,
                MarketPrice = 4.30m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Buy,
                Side = OrderSide.Buy,
                StockType = StockType.Lending
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "EA",
                Nvdr = false,
                AverageCostPrice = 4.02m,
                AvailableVolume = 200m,
                SellableVolume = 200m,
                Amount = 804.67m,
                IsNew = false,
                MarketPrice = 4.30m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Short,
                Side = OrderSide.Sell,
                StockType = StockType.Short
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "EA",
                Nvdr = false,
                AverageCostPrice = 0.00m,
                AvailableVolume = 1800m,
                SellableVolume = 1800m,
                Amount = 0.00m,
                IsNew = false,
                MarketPrice = 4.30m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Borrow,
                Side = OrderSide.Buy,
                StockType = StockType.Borrow
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "MINT",
                Nvdr = false,
                AverageCostPrice = 0.00m,
                AvailableVolume = 800m,
                SellableVolume = 800m,
                Amount = 0.00m,
                IsNew = false,
                MarketPrice = 26.00m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Borrow,
                Side = OrderSide.Buy,
                StockType = StockType.Borrow
            },
            new EquityAsset
            {
                AccountNo = "08010788",
                Symbol = "AE",
                Nvdr = false,
                AverageCostPrice = 4.02m,
                AvailableVolume = 200m,
                SellableVolume = 200m,
                Amount = 804.67m,
                IsNew = true,
                MarketPrice = 4.30m,
                RealizedPnl = 0.00m,
                Action = OrderAction.Short,
                Side = OrderSide.Sell,
                StockType = StockType.Short
            },
        ];
    }
}
