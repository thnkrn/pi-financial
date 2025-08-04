using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountInfo;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Factories;

public static class ApplicationFactory
{
    public static SetOrderInfo NewSetOder(BaseOrder order, string custCode)
    {
        return new SetOrderInfo
        {
            CustCode = custCode,
            Action = order.OrderAction,
            OrderId = order.OrderNo,
            BrokerOrderId = order.OrderNo,
            OrderNo = order.OrderNo.ToString(),
            Price = order.Price,
            Quantity = order.Volume,
            Symbol = order.SecSymbol,
            IsNVDR = order.IsNvdr(),
            ConditionPrice = order.ConditionPrice,
            Status = order.OrderStatus,
            QuantityExecuted = order.MatchVolume,
            AvgPrice = order.AvgMatchedPrice,
            OrderDateTime = order.OrderDateTime,
            OrderState = order.OrderState,
            CancelVolume = order.CancelVolume,
            Detail = order.Detail
        };
    }

    public static SblOrderInfo NewSblOrder(SblOrder sblOrder)
    {
        return new SblOrderInfo()
        {
            OrderId = (long)sblOrder.OrderId,
            CustCode = sblOrder.CustomerCode,
            Action = NewOrderAction(sblOrder.Type),
            Status = sblOrder.Status,
            AvgPrice = 0,
            BrokerOrderId = 0,
            IsNVDR = false,
            OrderNo = "0",
            Symbol = sblOrder.Symbol,
            Quantity = sblOrder.Volume,
            OrderDateTime = sblOrder.CreatedAt
        };
    }

    public static EquityAsset NewEquityAsset(AccountPosition accountPosition,
        EquityInstrument? equityInstrument)
    {
        return new EquityAsset
        {
            AccountNo = accountPosition.AccountNo,
            Symbol = accountPosition.SecSymbol,
            Nvdr = accountPosition.IsNvdr(),
            StockType = accountPosition.StockType,
            AvailableVolume = accountPosition.ActualVolume,
            SellableVolume = accountPosition.AvailableVolume,
            AverageCostPrice = accountPosition.AvgPrice,
            RealizedPnl = accountPosition.TodayRealize,
            MarketPrice = equityInstrument?.TradingDetail.Price ?? 0,
            Side = accountPosition.Side,
            Action = accountPosition.Action,
            IsNew = accountPosition.StockType == StockType.NewStockType8 ||
                    accountPosition.StockType == StockType.NewStockType82 ||
                    equityInstrument is { IsNew: true, TradingDetail.Price: 0 },
            Amount = accountPosition.Amount,
            CorporateActions = equityInstrument?.CorporateActions,
            InstrumentProfile = equityInstrument?.Profile
        };
    }

    public static CreditBalanceEquityAsset NewCreditBalanceEquityAsset(AccountPositionCreditBalance accountPosition,
        EquityInstrument? equityInstrument)
    {
        return new CreditBalanceEquityAsset
        {
            AccountNo = accountPosition.AccountNo,
            Symbol = accountPosition.SecSymbol,
            Nvdr = accountPosition.IsNvdr(),
            StockType = accountPosition.StockType,
            AvailableVolume = accountPosition.ActualVolume,
            SellableVolume = accountPosition.AvailableVolume,
            AverageCostPrice = accountPosition.AvgPrice,
            RealizedPnl = accountPosition.TodayRealize,
            MarketPrice = equityInstrument?.TradingDetail.Price ?? 0,
            Side = accountPosition.Side,
            Action = accountPosition.Action,
            IsNew = accountPosition.StockType == StockType.NewStockType8 ||
                    accountPosition.StockType == StockType.NewStockType82 ||
                    equityInstrument is { IsNew: true, TradingDetail.Price: 0 },
            Amount = accountPosition.Amount,
            CorporateActions = equityInstrument?.CorporateActions?.ToList(),
            InstrumentProfile = equityInstrument?.Profile,
            Mr = accountPosition.MR ?? 0
        };
    }

    public static CashAccountSummary NewCashAccountSummary(TradingAccount tradingAccount,
        AvailableCashBalance availableCashBalance,
        IEnumerable<EquityAsset> assets,
        BackofficeAvailableBalance? backofficeBalance)
    {
        return new CashAccountSummary
        {
            TradingAccountNo = availableCashBalance.TradingAccountNo,
            CustomerCode = tradingAccount.CustomerCode,
            AccountNo = availableCashBalance.AccountNo,
            TraderId = availableCashBalance.TraderId,
            CreditLimit = availableCashBalance.CreditLimit,
            BuyCredit = availableCashBalance.BuyCredit,
            CashBalance = availableCashBalance.CashBalance,
            AccountType = tradingAccount.TradingAccountType,
            Assets = assets,
            AsOfDate = DateTime.UtcNow,
            SblEnabled = tradingAccount.SblEnabled,
            Ar = availableCashBalance.Ar,
            Ap = availableCashBalance.Ap,
            ArTrade = availableCashBalance.ArTrade,
            ApTrade = availableCashBalance.ApTrade,
            TotalBuyMatch = availableCashBalance.TotalBuyMatch,
            TotalBuyUnmatch = availableCashBalance.TotalBuyUnmatch,
            CreditType = availableCashBalance.CreditType,
            BackofficeAvailableBalance = backofficeBalance
        };
    }

    public static CreditBalanceAccountSummary NewCreditBalanceAccountSummary(TradingAccount tradingAccount,
        AvailableCreditBalance creditBalance,
        IEnumerable<CreditBalanceEquityAsset> assets)
    {
        return new CreditBalanceAccountSummary
        {
            TradingAccountNo = creditBalance.TradingAccountNo,
            CustomerCode = tradingAccount.CustomerCode,
            AccountNo = creditBalance.AccountNo,
            TraderId = creditBalance.TraderId,
            CreditLimit = creditBalance.CreditLimit,
            BuyCredit = creditBalance.BuyCredit,
            CashBalance = creditBalance.CashBalance,
            AccountType = tradingAccount.TradingAccountType,
            Assets = assets,
            AsOfDate = DateTime.UtcNow,
            SblEnabled = tradingAccount.SblEnabled,
            Mr = creditBalance.MarginRequired,
            EquityValue = creditBalance.Equity,
            ExcessEquity = creditBalance.ExcessEquity,
            Liabilities = creditBalance.Liability,
            Lmv = creditBalance.Lmv ?? 0,
            Smv = creditBalance.Smv ?? 0
        };
    }

    public static CashAccountInfo NewCashAccountInfo(TradingAccount tradingAccount,
        AvailableCashBalance availableCashBalance,
        BackofficeAvailableBalance? backofficeBalance)
    {
        return new CashAccountInfo
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = availableCashBalance.AccountNo,
            TraderId = availableCashBalance.TraderId,
            Ar = availableCashBalance.Ar,
            Ap = availableCashBalance.Ap,
            ArTrade = availableCashBalance.ArTrade,
            ApTrade = availableCashBalance.ApTrade,
            TotalBuyMatch = availableCashBalance.TotalBuyMatch,
            TotalBuyUnmatch = availableCashBalance.TotalBuyUnmatch,
            CreditType = availableCashBalance.CreditType,
            CreditLimit = availableCashBalance.CreditLimit,
            BuyCredit = availableCashBalance.BuyCredit,
            CashBalance = availableCashBalance.CashBalance,
            BackofficeAvailableBalance = backofficeBalance
        };
    }

    public static CreditAccountInfo NewCreditAccountInfo(TradingAccount tradingAccount,
        AvailableCreditBalance availableCreditBalance)
    {
        return new CreditAccountInfo
        {
            TradingAccountNo = tradingAccount.TradingAccountNo,
            AccountNo = availableCreditBalance.AccountNo,
            Liability = availableCreditBalance.Liability,
            Asset = availableCreditBalance.Asset,
            Equity = availableCreditBalance.Equity,
            MarginRequired = availableCreditBalance.MarginRequired,
            ExcessEquity = availableCreditBalance.ExcessEquity,
            CallForce = availableCreditBalance.CallForce,
            CallMargin = availableCreditBalance.CallMargin,
            TraderId = availableCreditBalance.TraderId,
            CreditLimit = availableCreditBalance.CreditLimit,
            BuyCredit = availableCreditBalance.BuyCredit,
            CashBalance = availableCreditBalance.CashBalance
        };
    }

    public static OrderAction NewOrderAction(SblOrderType orderType)
    {
        return orderType switch
        {
            SblOrderType.Borrow => OrderAction.Borrow,
            SblOrderType.Return => OrderAction.Return,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }
}
