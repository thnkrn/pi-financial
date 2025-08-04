using Pi.Client.Sirius.Model;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.Application.Tests.Mock;

public static class MockSetTrade
{
    public static PortfolioResponse GeneratePortfolioResponse(bool? isLong = false, string symbol = "Symbol")
    {
        return new PortfolioResponse([
            GeneratePortfolio(isLong, symbol)
        ], GenerateTotalPortfolio());
    }

    public static TotalPortfolio GenerateTotalPortfolio()
    {
        return new TotalPortfolio(
            Amount: 0,
            MarketValue: 0,
            AmountByCost: 0,
            UnrealizePl: 0,
            UnrealizePlByCost: 0,
            RealizePl: 0,
            RealizePlByCost: 0,
            PercentUnrealizePl: 0,
            PercentUnrealizePlByCost: 0,
            OptionsValue: 0);
    }

    public static Portfolio GeneratePortfolio(bool? isLong = false, string symbol = "Symbol")
    {
        return new Portfolio(
            BrokerId: "BrokerId",
            AccountNo: "AccountNo",
            Symbol: symbol,
            Underlying: "Underlying",
            SecurityType: SecurityType.Futures,
            LastTradingDate: new DateOnly(2023, 12, 31),
            Multiplier: 100,
            Currency: Currency.THB,
            CurrentXrt: 1.0M,
            AsOfDateXrt: "2023-12-31",
            HasLongPosition: isLong ?? false,
            StartLongPosition: 100,
            ActualLongPosition: 100,
            AvailableLongPosition: 50,
            StartLongPrice: 100.0M,
            StartLongCost: 10000.0M,
            LongAvgPrice: 100.0M,
            LongAvgCost: 10000.0M,
            ShortAvgCostThb: 0M,
            LongAvgCostThb: 10000.0M,
            OpenLongPosition: 100,
            CloseLongPosition: 0,
            StartXrtLong: 1.0M,
            StartXrtLongCost: 10000.0M,
            AvgXrtLong: 1.0M,
            AvgXrtLongCost: 10000.0M,
            HasShortPosition: !isLong ?? false,
            StartShortPosition: 0,
            ActualShortPosition: 0,
            AvailableShortPosition: 0,
            StartShortPrice: 0,
            StartShortCost: 0,
            ShortAvgPrice: 0,
            ShortAvgCost: 0,
            OpenShortPosition: 0,
            CloseShortPosition: 0,
            StartXrtShort: 0,
            StartXrtShortCost: 0,
            AvgXrtShort: 0,
            AvgXrtShortCost: 0,
            MarketPrice: 100,
            RealizedPl: 0,
            RealizedPlByCost: 0,
            RealizedPlCurrency: 0,
            RealizedPlByCostCurrency: 0,
            ShortAmount: 0,
            LongAmount: 10000,
            ShortAmountByCost: 0,
            LongAmountByCost: 10000,
            PriceDigit: 2,
            SettleDigit: 2,
            LongUnrealizePl: 0,
            LongUnrealizePlByCost: 0,
            LongPercentUnrealizePl: 0,
            LongPercentUnrealizePlByCost: 0,
            LongOptionsValue: 0,
            LongMarketValue: 10000,
            ShortUnrealizePl: 0,
            ShortPercentUnrealizePl: 0,
            ShortUnrealizePlByCost: 0,
            ShortPercentUnrealizePlByCost: 0,
            ShortOptionsValue: 0,
            ShortMarketValue: 0,
            LongAvgPriceThb: 100,
            ShortAvgPriceThb: 0,
            ShortAmountCurrency: 0,
            LongAmountCurrency: 10000,
            LongMarketValueCurrency: 10000,
            ShortMarketValueCurrency: 0,
            LongUnrealizePlCurrency: 0,
            ShortUnrealizePlCurrency: 0,
            LongUnrealizedPlByCostCurrency: 0,
            ShortUnrealizedPlByCostCurrency: 0,
            LongAmountByCostCurrency: 10000,
            ShortAmountByCostCurrency: 0
        );
    }
}