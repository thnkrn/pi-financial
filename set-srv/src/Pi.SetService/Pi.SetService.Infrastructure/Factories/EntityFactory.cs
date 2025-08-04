using System.Globalization;
using Pi.Client.OnePort.GW.DB2.Model;
using Pi.Client.PiInternal.Model;
using Pi.SetService.Application.Extensions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using static Pi.Client.OnePort.GW.TCP.Model.PiOnePortTCPModelsPacketsDataTransferDataTransferOrderAcknowledgementResponse7K;

namespace Pi.SetService.Infrastructure.Factories;

public static class EntityFactory
{
    private const string DateFormat = "yyyyMMdd";
    private const string DateTimeFormat = "yyyyMMddHHmmss";

    public static OnlineOrder NewSetOnlineOrder(PiOnePortDb2ModelsOrder order)
    {
        var orderSide = GetOrderSide(order.Side);
        var orderType = GetOrderType(order.OrderType);

        return new OnlineOrder(order.OrderNo, order.AccountNo, CleanSymbol(order.SecSymbol), orderSide, NewTtf(order.TrusteeId))
        {
            TradingAccountNo = NewTradingAccountNo(order.AccountNo),
            Price = order.Price.GetRequiredValue(nameof(order.Price)),
            Volume = order.Volume.GetRequiredValue(nameof(order.Volume)),
            EnterId = order.EnterId,
            PubVolume = order.PubVolume,
            OrderStatus = GetOrderStatus(order.OrderStatus),
            OrderDateTime = NewUtcDateTime(order.OrderDate, order.OrderTime),
            ConditionPrice = GetConditionPrice(order.ConditionPrice),
            Type = orderType,
            OrderState = GetOrderState(order.OrderStatus),
            OrderAction = DomainFactory.NewOrderAction(orderSide, orderType),
            ServiceType = TryNewServiceType(order.ServiceType, out var serviceType) ? serviceType : null,
            MatchVolume = order.MatchVolume != null ? decimal.ToInt32((decimal)order.MatchVolume) : null
        };
    }

    public static OfflineOrder NewSetOfflineOrder(PiOnePortDb2ModelsOfflineOrder order)
    {
        var orderSide = GetOrderSide(order.Side);
        var orderType = GetOrderType(order.OrderType);

        return new OfflineOrder(order.OrderNo, order.AccountNo, CleanSymbol(order.SecSymbol), orderSide, NewTtf(order.TrusteeId))
        {
            TradingAccountNo = NewTradingAccountNo(order.AccountNo),
            Price = order.Price.GetRequiredValue(nameof(order.Price)),
            Volume = order.Volume.GetRequiredValue(nameof(order.Volume)),
            EnterId = order.EnterId,
            PubVolume = order.PubVolume,
            OrderStatus = GetOrderStatus(order.OrderStatus),
            OrderDateTime = NewUtcDateTime(order.OrderDate, order.OrderTime),
            ConditionPrice = GetConditionPrice(order.ConditionPrice),
            Type = orderType,
            OrderState = GetOrderState(order.OrderStatus,
                true),
            OrderAction = DomainFactory.NewOrderAction(orderSide,
                orderType),
            ServiceType = TryNewServiceType(order.ServiceType,
                out var serviceType)
                ? serviceType
                : null
        };
    }

    public static AvailableCashBalance NewAccountBalanceAvailable(
        PiOnePortDb2ModelsAccountAvailable accountBalanceAvailable)
    {
        return new AvailableCashBalance
        {
            TradingAccountNo = NewTradingAccountNo(accountBalanceAvailable.AccountNo),
            AccountNo = accountBalanceAvailable.AccountNo,
            TraderId = accountBalanceAvailable.TraderId,
            CreditLimit = accountBalanceAvailable.CreditLimit ?? 0,
            BuyCredit = accountBalanceAvailable.BuyCredit ?? 0,
            CashBalance = accountBalanceAvailable.CashBalance ?? 0,
            Ar = accountBalanceAvailable.Ar ?? 0,
            Ap = accountBalanceAvailable.Ap ?? 0,
            ArTrade = accountBalanceAvailable.ArTrade ?? 0,
            ApTrade = accountBalanceAvailable.ApTrade ?? 0,
            TotalBuyMatch = accountBalanceAvailable.TotalBuyMatch ?? 0,
            TotalBuyUnmatch = accountBalanceAvailable.TotalBuyUnmatch ?? 0,
            CreditType = accountBalanceAvailable.CreditType
        };
    }

    public static AvailableCreditBalance NewAccountAvailableCreditBalance(
        PiOnePortDb2ModelsAccountAvailableCreditBalance accountCredit)
    {
        return new AvailableCreditBalance
        {
            TradingAccountNo = NewTradingAccountNo(accountCredit.CustId),
            AccountNo = accountCredit.CustId,
            TraderId = accountCredit.TraderId,
            CreditLimit = accountCredit.CreditLimit.GetRequiredValue(nameof(accountCredit.CreditLimit)),
            BuyCredit = accountCredit.BuyCredit.GetRequiredValue(nameof(accountCredit.BuyCredit)),
            CashBalance = accountCredit.CashBalance.GetRequiredValue(nameof(accountCredit.CashBalance)),
            Liability = accountCredit.Liability.GetRequiredValue(nameof(accountCredit.Liability)),
            Asset = accountCredit.Asset.GetRequiredValue(nameof(accountCredit.Asset)),
            Equity = accountCredit.Equity.GetRequiredValue(nameof(accountCredit.Equity)),
            MarginRequired = accountCredit.Mr.GetRequiredValue(nameof(accountCredit.Mr)),
            ExcessEquity = accountCredit.Ee.GetRequiredValue(nameof(accountCredit.Ee)),
            Pc = GetPc(accountCredit.Pc),
            Lmv = accountCredit.Lmv,
            Collat = accountCredit.Collat,
            Debt = accountCredit.Debt,
            Smv = accountCredit.Smv,
            BuyMR = accountCredit.BuyMR,
            SellMR = accountCredit.SellMR,
            Pp = accountCredit.Pp,
            CallMargin = accountCredit.CallMargin.GetRequiredValue(nameof(accountCredit.CallMargin)),
            CallForce = accountCredit.CallForce.GetRequiredValue(nameof(accountCredit.CallForce)),
            BrkCallLMV = accountCredit.BrkCallLMV,
            BrkCallSMV = accountCredit.BrkCallSMV,
            BrkSellLMV = accountCredit.BrkSellLMV,
            BrkSellSMV = accountCredit.BrkSellSMV,
            Action = accountCredit.Action,
            BuyUnmatch = accountCredit.BuyUnmatch,
            SellUnmatch = accountCredit.SellUnmatch,
            Ar = accountCredit.Ar,
            Ap = accountCredit.Ap,
            ArT1 = accountCredit.ArT1,
            ApT1 = accountCredit.ApT1,
            ArT2 = accountCredit.ArT2,
            ApT2 = accountCredit.ApT2,
            Withdrawal = accountCredit.Withdrawal,
            LmvHaircut = accountCredit.LmvHaircut,
            EquityHaircut = accountCredit.EquityHaircut,
            EE1 = accountCredit.EE1,
            EE50 = accountCredit.EE50,
            EE60 = accountCredit.EE60,
            EE70 = accountCredit.EE70,
            Eemtm = accountCredit.Eemtm,
            EemtM50 = accountCredit.EemtM50,
            EemtM60 = accountCredit.EemtM60,
            EemtM70 = accountCredit.EemtM70,
            DelFlag = ConvertFlag(accountCredit.DelFlag),
            UpdateFlag = ConvertFlag(accountCredit.UpdateFlag)
        };
    }

    public static AccountPosition NewAccountPosition(PiOnePortDb2ModelsAccountPosition accountPosition)
    {
        return new AccountPosition(CleanSymbol(accountPosition.SecSymbol), NewTtf(accountPosition.TrusteeId))
        {
            TradingAccountNo = NewTradingAccountNo(accountPosition.AccountNo),
            AccountNo = accountPosition.AccountNo,
            StockType = GetStockType(accountPosition.StockType),
            StockTypeChar = GetStockTypeChar(accountPosition.StockTypeChar),
            StartVolume = accountPosition.StartVolume.GetRequiredValue(nameof(accountPosition.StartVolume)),
            StartPrice = accountPosition.StartPrice.GetRequiredValue(nameof(accountPosition.StartPrice)),
            AvailableVolume = accountPosition.AvaiVolume.GetRequiredValue(nameof(accountPosition.AvaiVolume)),
            AvgPrice = accountPosition.AvgPrice.GetRequiredValue(nameof(accountPosition.AvgPrice)),
            TodayRealize = accountPosition.TodayRealize,
            Amount = accountPosition.Amount.GetRequiredValue(nameof(accountPosition.Amount)),
            ActualVolume = accountPosition.ActualVolume.GetRequiredValue(nameof(accountPosition.ActualVolume))
        };
    }

    public static AccountPositionCreditBalance NewAccountPosition(
        PiOnePortDb2ModelsAccountPositionCreditBalance accountPosition)
    {
        return new AccountPositionCreditBalance(CleanSymbol(accountPosition.SecSymbol), NewTtf(accountPosition.TrusteeId))
        {
            TradingAccountNo = NewTradingAccountNo(accountPosition.AccountNo),
            AccountNo = accountPosition.AccountNo,
            StockType = GetStockType(accountPosition.StockType),
            StockTypeChar = GetStockTypeChar(accountPosition.StockTypeChar),
            StartVolume = accountPosition.StartVolume.GetRequiredValue(nameof(accountPosition.StartVolume)),
            StartPrice = accountPosition.StartPrice.GetRequiredValue(nameof(accountPosition.StartPrice)),
            AvailableVolume = accountPosition.AvaiVolume.GetRequiredValue(nameof(accountPosition.AvaiVolume)),
            AvgPrice = accountPosition.AvgPrice.GetRequiredValue(nameof(accountPosition.AvgPrice)),
            TodayRealize = accountPosition.TodayRealize,
            Amount = accountPosition.Amount.GetRequiredValue(nameof(accountPosition.Amount)),
            ActualVolume = accountPosition.ActualVolume.GetRequiredValue(nameof(accountPosition.ActualVolume)),
            LastSale = accountPosition.LastSale,
            MktValue = accountPosition.MktValue,
            MR = accountPosition.Mr,
            Grade = accountPosition.Grade,
            AvgCost = accountPosition.AvgPrice,
            TodayMargin = accountPosition.TodayMargin,
            StartClose = accountPosition.StartClose,
            UpdateFlag = ConvertFlag(accountPosition.UpdateFlag),
            DelFlag = ConvertFlag(accountPosition.DelFlag)
        };
    }

    private static string NewTradingAccountNo(string accountNo)
    {
        return accountNo.Insert(accountNo.Length - 1, "-");
    }

    private static bool ConvertFlag(string flag)
    {
        return flag.Equals("y", StringComparison.CurrentCultureIgnoreCase);
    }

    private static string CleanSymbol(string symbol)
    {
        return symbol.Replace(":", "");
    }

    private static Ttf NewTtf(string trusteeId)
    {
        return trusteeId switch
        {
            "1" => Ttf.TrustFund,
            "2" => Ttf.Nvdr,
            _ => Ttf.None
        };
    }

    public static Deal NewDeal(PiOnePortDb2ModelsAccountDeal deal)
    {
        return new Deal(deal.OrderNo, deal.ConfirmNo)
        {
            DealVolume = deal.DealVolume.GetRequiredValue(nameof(deal.DealVolume)),
            DealPrice = deal.DealPrice.GetRequiredValue(nameof(deal.DealPrice)),
            DealDateTime = NewUtcDateTime(deal.DealDate, deal.DealTime),
            SumComm = deal.SumComm ?? 0,
            SumVat = deal.SumVat ?? 0,
            SumTradingFee = deal.SumTradingFee ?? 0,
            SumClearingFee = deal.SumClearingFee ?? 0
        };
    }

    private static DateTime NewUtcDateTime(string date, string time)
    {
        var dateTime = DateTime.ParseExact(date + time.Replace(":", ""), DateTimeFormat, CultureInfo.InvariantCulture);

        return DateTimeHelper.ConvertThTimeToUtc(dateTime);
    }

    public static TradingAccountType? NewTradingAccountType(string accountTypeCode, bool throwException = true)
    {
        return accountTypeCode switch
        {
            "CC" => TradingAccountType.Cash,
            "CB" => TradingAccountType.CreditBalance,
            "CH" => TradingAccountType.CashBalance,
            _ => throwException ? throw new ArgumentOutOfRangeException(accountTypeCode) : null
        };
    }

    private static OrderSide GetOrderSide(string side)
    {
        return side switch
        {
            "B" => OrderSide.Buy,
            "S" => OrderSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, string.Empty)
        };
    }

    private static Pc? GetPc(string? pc)
    {
        return pc switch
        {
            "B" => Pc.B,
            "T" => Pc.T,
            // Not sure about possible values [TBC]
            _ => null
        };
    }

    private static ConditionPrice GetConditionPrice(string conPrice)
    {
        return conPrice switch
        {
            "" => ConditionPrice.Limit,
            " " => ConditionPrice.Limit,
            "A" => ConditionPrice.Ato,
            "C" => ConditionPrice.Atc,
            "M" => ConditionPrice.Mp,
            "K" => ConditionPrice.Mkt,
            "L" => ConditionPrice.Mtl,
            _ => throw new ArgumentOutOfRangeException(nameof(conPrice), conPrice, string.Empty)
        };
    }

    private static OrderStatus GetOrderStatus(string? orderStatus)
    {
        return orderStatus switch
        {
            "Pending" => OrderStatus.Pending,
            "PO" => OrderStatus.Pending,
            "PC" => OrderStatus.Pending,
            "A" => OrderStatus.Pending,
            "POA" => OrderStatus.Pending,
            "PX" => OrderStatus.Pending,
            "PXA" => OrderStatus.Pending,
            "PXC" => OrderStatus.Pending,
            "O" => OrderStatus.Pending,
            "m" => OrderStatus.Pending,
            "OA" => OrderStatus.Pending,
            "OAC" => OrderStatus.Pending,
            "OC" => OrderStatus.Pending,

            "M" => OrderStatus.Matched,

            "MA" => OrderStatus.PartialMatch,
            "MAC" => OrderStatus.PartialMatch,
            "MPC" => OrderStatus.PartialMatch,
            "MD" => OrderStatus.PartialMatch,
            "MC" => OrderStatus.PartialMatch,
            "MDC" => OrderStatus.PartialMatch,

            "X" => OrderStatus.Cancelled,
            "XA" => OrderStatus.Cancelled,
            "XC" => OrderStatus.Cancelled,

            "R" => OrderStatus.Rejected,
            "C" => OrderStatus.Rejected,
            _ => OrderStatus.Unknown
        };
    }

    private static OrderState GetOrderState(string? orderStatus, bool? isOffline = false)
    {
        return orderStatus switch
        {
            "O" => isOffline == true ? OrderState.OF : OrderState.O,
            "m" => OrderState.m,
            "M" => OrderState.M,
            _ => Enum.TryParse<OrderState>(orderStatus, out var state) ? state : throw new ArgumentOutOfRangeException(nameof(orderStatus), orderStatus, string.Empty)
        };
    }

    private static OrderType GetOrderType(string? orderType)
    {
        return orderType switch
        {
            "S" => OrderType.ShortCover,
            "R" => OrderType.SellLending,
            "P" => OrderType.Program,
            "G" => OrderType.MarketProgram,
            "M" => OrderType.Market,
            _ => OrderType.Normal
        };
    }

    private static StockType GetStockType(int? stockType)
    {
        return stockType switch
        {
            2 => StockType.Normal,
            20 => StockType.Short,
            22 => StockType.Borrow,
            23 => StockType.Lending,
            8 => StockType.NewStockType8,
            82 => StockType.NewStockType82,
            _ => StockType.Unknow
        };
    }

    private static StockTypeChar GetStockTypeChar(string stockTypeChar)
    {
        //TODO: Need to investigate later the possible value and map
        return stockTypeChar switch
        {
            " " => StockTypeChar.Unknown,
            "H" => StockTypeChar.Unknown,
            _ => StockTypeChar.Unknown
        };
    }

    private static CustomerType GetCustomerType(string custType)
    {
        return custType switch
        {
            "C" => CustomerType.BrokerClient,
            "P" => CustomerType.Broker,
            "F" => CustomerType.BrokerForeign,
            "I" => CustomerType.SubBrokerClient,
            "M" => CustomerType.MutualBroker,
            "S" => CustomerType.SubBrokerPortfolio,
            "O" => CustomerType.SubBrokerForeign,
            "U" => CustomerType.SubBrokerMutualFund,
            _ => throw new ArgumentOutOfRangeException(nameof(custType), custType, null)
        };
    }

    private static TradingAccountType GetAccountType(string accType)
    {
        return accType switch
        {
            "C" => TradingAccountType.Cash,
            "A" => TradingAccountType.CashMargin,
            "M" => TradingAccountType.MaintenanceMargin,
            "B" => TradingAccountType.CreditBalance,
            "H" => TradingAccountType.CashBalance,
            "I" => TradingAccountType.Internet,
            _ => throw new ArgumentOutOfRangeException(nameof(accType), accType, null)
        };
    }

    public static BrokerOrderStatus NewBrokerOrderStatus(
        OrderStatusEnum status)
    {
        return status switch
        {
            OrderStatusEnum.Accepted => BrokerOrderStatus.Accepted,
            OrderStatusEnum.Warning => BrokerOrderStatus.Warning,
            OrderStatusEnum.Rejected => BrokerOrderStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static ExecutionTransType NewExecutionTransType(
        ExecutionTransTypeEnum executionTransType)
    {
        return executionTransType switch
        {
            ExecutionTransTypeEnum.New => ExecutionTransType.New,
            ExecutionTransTypeEnum.Cancel => ExecutionTransType.Cancel,
            ExecutionTransTypeEnum.ChangeAcct => ExecutionTransType.Change,
            _ => throw new ArgumentOutOfRangeException(nameof(executionTransType), executionTransType, null)
        };
    }

    public static ExecutionTransRejectType NewExecutionTransRejectType(
        ExecutionTransRejectTypeEnum? executionTransRejectType)
    {
        return executionTransRejectType switch
        {
            ExecutionTransRejectTypeEnum.Fis => ExecutionTransRejectType.Fis,
            ExecutionTransRejectTypeEnum.Set => ExecutionTransRejectType.Set,
            _ => throw new ArgumentNullException(executionTransRejectType.ToString())
        };
    }

    public static Source NewSource(SourceEnum source)
    {
        return source switch
        {
            SourceEnum.Fis => Source.Fis,
            SourceEnum.Set => Source.Set,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private static bool TryNewServiceType(string orderServiceType, out ServiceType? serviceType)
    {
        ServiceType? result = orderServiceType switch
        {
            "D" => ServiceType.Dealer,
            "I" => ServiceType.Internet,
            "V" => ServiceType.Vip,
            _ => null
        };

        serviceType = result;
        return result != null;
    }

    private static OrderAction GetOrderAction(string type)
    {
        return type switch
        {
            "BU" => OrderAction.Buy,
            "SE" => OrderAction.Sell,
            "SH" => OrderAction.Short,
            "BH" => OrderAction.Borrow,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static Trade NewTrade(TradeDetail tradeDetail)
    {
        var dealDateTime = NewUtcDateTime(tradeDetail.RefDate.ToString(DateFormat), tradeDetail.ConfirmTimeString);

        return new Trade
        {
            DealDateTime = dealDateTime,
            AccountNo = tradeDetail.Account.Replace("-", ""),
            Symbol = tradeDetail.ShareCode,
            Side = GetOrderAction(tradeDetail.RefType),
            Price = (decimal)tradeDetail.Price,
            Volume = (int)tradeDetail.Unit,
            TotalAmount = (decimal)tradeDetail.Amt,
            CommSub = (decimal)tradeDetail.CommSub,
            VatSub = (decimal)tradeDetail.VatSub,
            TradeFee = (decimal)tradeDetail.TradeFee,
            ClrFee = (decimal)tradeDetail.ClrFee,
            SecFee = (decimal)tradeDetail.SecFee,
            RegFee = (decimal)tradeDetail.RegFee
        };
    }
}
