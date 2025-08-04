using Pi.Common.ExtensionMethods;
using Pi.SetService.API.Extensions;
using Pi.SetService.API.Models;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountInfo;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using CaEvent = Pi.SetService.API.Models.CaEvent;

namespace Pi.SetService.API.Factories;

public static class SetFactory
{
    private const string VenueCodeEquity = "Equity";
    public const string DateFormat = "yyyyMMdd";
    private const decimal Multiplier = 100.0m;

    public static SetOpenOrderResponse NewOpenOrderResponse(SetOrderInfo order)
    {
        return new SetOpenOrderResponse
        {
            OrderId = order.OrderId,
            CustCode = order.CustCode,
            Side = order.Action,
            Status = order.Status.GetEnumDescription(),
            OrderType = order.ConditionPrice.GetEnumDescription(),
            OrderTimeStamp = order.OrderDateTime == null
                ? null
                : DateTimeHelper.ToUnixTime((DateTime)order.OrderDateTime),
            Amount = order.Quantity.Round(),
            Price = order.Price.Round(),
            QuantityExecuted = order.QuantityExecuted,
            AvgPrice = order.AvgPrice.Round(),
            BrokerOrderId = order.BrokerOrderId,
            MatchedPrice = (Math.Round(order.AvgPrice * Multiplier) / Multiplier).Round(),
            IsNVDR = order.IsNVDR,
            InterestRate = order.InterestRate == null ? null : order.InterestRate.Round(),
            Logo = order.InstrumentProfile?.Logo,
            FriendlyName = order.InstrumentProfile?.FriendlyName,
            InstrumentCategory = order.InstrumentProfile?.InstrumentCategory,
            OrderNo = order.OrderNo,
            Symbol = order.Symbol
        };
    }

    public static SetOpenOrderResponse NewOpenOrderResponse(SblOrderInfo orderInfo)
    {
        return new SetOpenOrderResponse
        {
            OrderId = orderInfo.OrderId,
            CustCode = orderInfo.CustCode,
            Side = orderInfo.Action,
            Status = orderInfo.Status.GetEnumDescription(),
            OrderType = orderInfo.Action.GetEnumDescription(),
            OrderTimeStamp = orderInfo.OrderDateTime == null
                ? null
                : DateTimeHelper.ToUnixTime((DateTime)orderInfo.OrderDateTime),
            Amount = orderInfo.Quantity.Round(),
            AvgPrice = orderInfo.AvgPrice.Round(),
            BrokerOrderId = orderInfo.BrokerOrderId,
            IsNVDR = orderInfo.IsNVDR,
            InterestRate = orderInfo.InterestRate == null
                ? null
                : orderInfo.InterestRate.Round(),
            Logo = orderInfo.InstrumentProfile?.Logo,
            FriendlyName = orderInfo.InstrumentProfile?.FriendlyName,
            InstrumentCategory = orderInfo.InstrumentProfile?.InstrumentCategory,
            OrderNo = orderInfo.OrderNo,
            Symbol = orderInfo.Symbol,
            MatchedPrice = 0,
            Price = 0
        };
    }

    public static SetOrderHistoryResponse NewOrderHistoryResponse(SetOrderInfo order)
    {
        return new SetOrderHistoryResponse
        {
            OrderId = order.OrderId,
            IsNVDR = order.IsNVDR,
            Status = order.Status.GetEnumDescription(),
            Symbol = order.Symbol,
            Side = order.Action,
            TradeType = order.ConditionPrice.GetEnumDescription(),
            OrderTimeStamp = order.OrderDateTime == null
                ? null
                : DateTimeHelper.ToUnixTime((DateTime)order.OrderDateTime),
            Amount = order.Quantity.Round(),
            Price = order.Price.Round(),
            OrderNo = order.OrderId,
            MatchedPrice = Math.Round(order.AvgPrice * Multiplier) / Multiplier,
            InterestRate = order.InterestRate == null ? null : order.InterestRate.Round(),
            RealizedPL = order.RealizedPL,
            MatchVolume = order.QuantityExecuted,
            Detail = order.Detail,
            CancelVolume = order.CancelVolume == null ? null : order.CancelVolume.Round()
        };
    }

    public static SetOrderHistoryResponse NewOrderHistoryResponse(SblOrderInfo orderInfo)
    {
        return new SetOrderHistoryResponse
        {
            OrderId = orderInfo.OrderId,
            IsNVDR = orderInfo.IsNVDR,
            Symbol = orderInfo.Symbol,
            Side = orderInfo.Action,
            OrderTimeStamp = orderInfo.OrderDateTime == null
                ? null
                : DateTimeHelper.ToUnixTime((DateTime)orderInfo.OrderDateTime),
            Amount = orderInfo.Quantity.Round(),
            OrderNo = orderInfo.OrderId,
            MatchedPrice = 0,
            InterestRate = orderInfo.InterestRate == null
                ? null
                : orderInfo.InterestRate.Round(),
            Status = orderInfo.Status.GetEnumDescription(),
            TradeType = orderInfo.Action.GetEnumDescription(),
            Price = 0
        };
    }

    public static SetTradeHistoryResponse NewTradeHistoryResponse(Trade trade)
    {
        var totalCommissionAndVat =
            trade.CommSub + trade.VatSub + trade.TradeFee + trade.ClrFee + trade.SecFee + trade.RegFee;
        return new SetTradeHistoryResponse
        {
            AccountCode = trade.AccountNo,
            Currency = "THB",
            InstrumentSymbol = trade.Symbol,
            Side = trade.Side.GetEnumDescription(),
            TradeDate = trade.DealDateTime.ToString("yyyy-MM-dd"),
            TradeTime = trade.DealDateTime.ToString("HH:mm:ss"),
            Price = trade.Price,
            Volume = trade.Volume,
            TotalAmount = trade.TotalAmount,
            Amount = trade.Volume * trade.Price,
            TotalCommissionAndVat = totalCommissionAndVat
        };
    }

    public static SetAccountAssetsResponse NewSetEquityAssetResponse(EquityAsset equityAsset)
    {
        return new SetAccountAssetsResponse
        {
            Symbol = equityAsset.Symbol,
            MarketPrice = equityAsset.MarketPrice,
            Side = equityAsset.Action.GetEnumDescription(),
            Nvdr = equityAsset.Nvdr,
            StockType = ((int)equityAsset.StockType).ToString("D2"),
            AssetType = VenueCodeEquity,
            IsNew = equityAsset.IsNew,
            CaFlag = equityAsset.CorporateActions != null && equityAsset.CorporateActions.Any(),
            AverageCostPrice = equityAsset.AverageCostPrice.Round(2),
            CostValue = equityAsset.CostValue.Round(2),
            MarketValue = equityAsset.MarketValue.Round(2),
            Upnl = equityAsset.Upnl.Round(2),
            UpnlPercentage = equityAsset.UpnlPercentage.Round(2),
            RealizedPnl = equityAsset.RealizedPnl == null ? null : equityAsset.RealizedPnl.Round(2),
            AvailableVolume = equityAsset.AvailableVolume.Round(2),
            SellableVolume = equityAsset.SellableVolume.Round(2),
            LendingVolume = equityAsset.LendingVolume.Round(2),
            Logo = equityAsset.InstrumentProfile?.Logo,
            FriendlyName = equityAsset.InstrumentProfile?.FriendlyName,
            InstrumentCategory = equityAsset.InstrumentProfile?.InstrumentCategory,
            CaEventList = equityAsset.CorporateActions?.Select(NewSetCompanyActivityResponse).ToList() ?? []
        };
    }

    public static SetAccountCreditBalanceInfoResponse NewSetAccountCreditBalanceInfoResponse(
        CreditAccountInfo creditAccountInfo)
    {
        return new SetAccountCreditBalanceInfoResponse
        {
            AccountNo = creditAccountInfo.AccountNo,
            BuyLimit = creditAccountInfo.BuyCredit.Round(2),
            CreditLimit = creditAccountInfo.CreditLimit.Round(2),
            ExcessEquity = creditAccountInfo.ExcessEquity.Round(2),
            MarginRequired = creditAccountInfo.MarginRequired.Round(2),
            MarginRatio = creditAccountInfo.MmPercentage.Round(2),
            CashBalance = creditAccountInfo.CashBalance.Round(2),
            Liabilities = creditAccountInfo.Liability.Round(2),
            CallForce = creditAccountInfo.CallForce.Round(2),
            MarginCall = creditAccountInfo.CallMargin.Round(2)
        };
    }

    public static SetAccountCashBalanceInfoResponse NewSetAccountBalanceInfoResponse(
        CashAccountInfo cashAccountInfo)
    {
        return new SetAccountCashBalanceInfoResponse
        {
            AccountNo = cashAccountInfo.AccountNo,
            BuyLimit = cashAccountInfo.BuyCredit.Round(2),
            CreditLimit = cashAccountInfo.CreditLimit.Round(2),
            CashBalance = cashAccountInfo.CashBalance.Round(2),
            PendingSettlement = cashAccountInfo.PendingSettlement.Round(2)
        };
    }

    public static SetAccountSummaryResponse NewSetAccountSummaryResponse(AccountSummary accountSummary)
    {
        return new SetAccountSummaryResponse
        {
            TradingAccountNo = accountSummary.TradingAccountNo,
            TradingAccountType = accountSummary.AccountType,
            CustomerCode = accountSummary.CustomerCode,
            AccountCode = accountSummary.AccountNo,
            CashBalance = accountSummary.CashBalance.Round(),
            AsOfDate = accountSummary.AsOfDate.ToString(DateFormat),
            TotalCost = accountSummary.TotalCost.Round(),
            TotalUpnl = accountSummary.TotalUpnl.Round(),
            TotalValue = accountSummary.TotalValue.Round(),
            TotalMarketValue = accountSummary.TotalMarketValue.Round(),
            TotalUpnlPercentage = accountSummary.TotalUpnlPercentage.Round(),
            SblEnabled = accountSummary.SblEnabled,
            Assets = accountSummary.Assets.Select(NewSetEquityAssetResponse)
                .ToList()
        };
    }

    public static SetCashAccountSummaryResponse NewSetCashAccountSummaryResponse(CashAccountSummary accountSummary)
    {
        return new SetCashAccountSummaryResponse
        {
            TradingAccountNo = accountSummary.TradingAccountNo,
            TradingAccountType = accountSummary.AccountType,
            CustomerCode = accountSummary.CustomerCode,
            AccountCode = accountSummary.AccountNo,
            CashBalance = accountSummary.CashBalance.Round(),
            TotalValue = accountSummary.TotalValue.Round(),
            AsOfDate = accountSummary.AsOfDate.ToString(DateFormat),
            TotalCost = accountSummary.TotalCost.Round(),
            TotalUpnl = accountSummary.TotalUpnl.Round(),
            TotalUpnlPercentage = accountSummary.TotalUpnlPercentage.Round(),
            LineAvailable = accountSummary.BuyCredit.Round(),
            TotalMarketValue = accountSummary.TotalMarketValue.Round(),
            SblEnabled = accountSummary.SblEnabled,
            Assets = accountSummary.Assets.Select(NewSetEquityAssetResponse)
                .ToList()
        };
    }

    public static SetCreditBalanceAccountSummaryResponse NewSetCreditBalanceAccountSummaryResponse(
        CreditBalanceAccountSummary accountSummary)
    {
        return new SetCreditBalanceAccountSummaryResponse
        {
            TradingAccountNo = accountSummary.TradingAccountNo,
            TradingAccountType = accountSummary.AccountType,
            CustomerCode = accountSummary.CustomerCode,
            AccountCode = accountSummary.AccountNo,
            CashBalance = accountSummary.CashBalance.Round(),
            AsOfDate = accountSummary.AsOfDate.ToString(DateFormat),
            TotalCost = accountSummary.TotalCost.Round(),
            TotalUpnl = accountSummary.TotalUpnl.Round(),
            TotalUpnlPercentage = accountSummary.TotalUpnlPercentage.Round(),
            TotalValue = accountSummary.TotalValue.Round(),
            TotalMarketValue = accountSummary.TotalMarketValue.Round(),
            SblEnabled = accountSummary.SblEnabled,
            Assets = accountSummary.Assets.Select(NewSetEquityAssetResponse)
                .ToList(),
            AccountValue = accountSummary.TotalValue.Round(),
            LongMarketValue = accountSummary.LongMarketValue.Round(),
            ShortMarketValue = accountSummary.ShortMarketValue.Round(),
            MarginLoan = accountSummary.MarginLoan.Round(),
            LongCostValue = accountSummary.LongCostValue.Round(),
            ShortCostValue = accountSummary.ShortCostValue.Round(),
            ExcessEquity = accountSummary.ExcessEquity.Round(),
            Liabilities = accountSummary.Liabilities.Round()
        };
    }

    private static CaEvent NewSetCompanyActivityResponse(CorporateAction corporateAction)
    {
        return new CaEvent
        {
            Date = corporateAction.Date.ToString(DateFormat),
            CaType = corporateAction.CaType
        };
    }

    public static MarginRateResponse NewMarginRateResponse(EquityMarginInfo equityMarginInfo)
    {
        return new MarginRateResponse
        {
            Symbol = equityMarginInfo.Symbol,
            Rate = equityMarginInfo.Rate.Round(),
            IsTurnoverList = equityMarginInfo.IsTurnoverList
        };
    }

    public static AccountInstrumentAvailableBalanceResponse NewAccountInstrumentAvailableBalanceResponse(
        AccountInstrumentBalance balance)
    {
        return new AccountInstrumentAvailableBalanceResponse
        {
            AccountDetail = new AccountDetail
            {
                BalanceUnit = balance.BalanceUnit,
                Balance = balance.Balance
            },
            UnitRemain = new UnitRemain
            {
                AssetSymbol = balance.Symbol,
                Unit = balance.Unit,
                NvdrUnit = balance.NvdrUnit,
                ShortUnit = balance.ShortUnit,
                ShortNvdrUnit = balance.ShortNvdrUnit
            }
        };
    }

    public static AccountSblInstrumentAvailableBalanceResponse NewAccountSblInstrumentAvailableBalanceResponse(
        AccountSblInstrumentBalance balance)
    {
        return new AccountSblInstrumentAvailableBalanceResponse
        {
            Ee = balance.ExcessEquity,
            ShelfAvailable = decimal.ToInt32(balance.AvailableLending),
            SblFlag = balance.SblEnabled,
            BorrowCredit = balance.PurchesingPower,
            MaximumShares = balance.MaximumShares,
            ShortUnitAvailable = balance.BorrowUnit,
            Power = balance.PurchesingPower,
            InitialMarginRate = balance.MarginRate,
            ClosePriceYesterday = balance.ClosePrice,
            AllowBorrowing = balance.AllowBorrowing
        };
    }
}
