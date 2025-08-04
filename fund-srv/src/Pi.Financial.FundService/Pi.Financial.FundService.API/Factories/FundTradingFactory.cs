using System.Globalization;
using System.Text.Json;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.API.Factories;

public static class FundTradingFactory
{
    public const string DateFormat = "yyyyMMdd";
    public const string DateTimeFormat = "yyyyMMddHHmmss";

    public static SiriusFundAssetResponse NewFundAssetResponse(FundAsset fundAsset)
    {
        return new SiriusFundAssetResponse
        {
            AverageCostPrice = decimal.Round(fundAsset.AvgCostPrice, 4),
            AsOfDate = fundAsset.AsOfDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            RemainUnit = decimal.Round(fundAsset.RemainUnit, 4),
            Symbol = fundAsset.FundCode,
            FriendlyName = fundAsset.Info?.Name,
            UpnlPercentage = decimal.Round(fundAsset.UPNLPercentage, 4),
            MarketPrice = decimal.Round(fundAsset.MarketPrice, 4),
            CostValue = decimal.Round(fundAsset.CostValue, 4),
            Logo = fundAsset.Info?.Logo,
            Upnl = decimal.Round(fundAsset.UPNL, 4),
            AvailableVolume = decimal.Round(fundAsset.Unit, 4),
            InstrumentCategory = fundAsset.Info?.InstrumentCategory,
            MarketValue = decimal.Round(fundAsset.MarketValue, 4),
            CustCode = fundAsset.CustCode,
            UnitHolderId = fundAsset.UnitHolderId,
            RemainAmount = decimal.Round(fundAsset.RemainAmount, 4),
        };
    }
    public static InternalFundAssetResponse NewInternalFundAssetResponse(FundAsset fundAsset)
    {
        return new InternalFundAssetResponse
        {
            AsOfDate = fundAsset.AsOfDate,
            CustCode = fundAsset.CustCode,
            TradingAccountNo = fundAsset.TradingAccountNo,
            UnitHolderId = fundAsset.UnitHolderId,
            FundCode = fundAsset.FundCode,
            FriendlyName = fundAsset.Info?.Name,
            InstrumentCategory = fundAsset.Info?.InstrumentCategory,
            AvgCostPrice = decimal.Round(fundAsset.AvgCostPrice,
                4),
            MarketPrice = decimal.Round(fundAsset.MarketPrice,
                4),
            CostValue = decimal.Round(fundAsset.CostValue,
                4),
            Unit = decimal.Round(fundAsset.Unit,
                4),
            MarketValue = decimal.Round(fundAsset.MarketValue,
                4),
            RemainAmount = decimal.Round(fundAsset.RemainAmount, 4),
            RemainUnit = decimal.Round(fundAsset.RemainUnit, 4),
            PendingAmount = decimal.Round(fundAsset.PendingAmount, 4),
            PendingUnit = decimal.Round(fundAsset.PendingUnit, 4),
            Upnl = decimal.Round(fundAsset.UPNL,
                4),
            UpnlPercentage = decimal.Round(fundAsset.UPNLPercentage,
                4)
        };
    }

    public static SiriusFundOrderResponse NewFundOrderResponse(FundOrder fundOrder)
    {
        return new SiriusFundOrderResponse
        {
            Unit = decimal.Round(fundOrder.Unit ?? 0, 4),
            Amount = decimal.Round(fundOrder.Amount ?? 0, 4),
            PaymentMethod = fundOrder.PaymentMethod,
            BankAccount = fundOrder.BankAccount ?? "",
            Edd = fundOrder.Edd,
            SwitchIn = fundOrder.SwitchIn,
            SwitchTo = fundOrder.SwitchTo,
            BankCode = fundOrder.BankCode ?? "",
            BankShortName = fundOrder.BankInfo?.ShortName!,
            OrderId = fundOrder.BrokerOrderId,
            Account = fundOrder.AccountId,
            FundCode = fundOrder.FundCode,
            OrderType = fundOrder.OrderType.ToDescriptionString(),
            Status = NewSiriusFundStatus(fundOrder.Status),
            Nav = fundOrder.FundInfo?.Nav == null ? null : decimal.Round(fundOrder.FundInfo?.Nav ?? 0, 4),
            TransactionLastUpdated = fundOrder.TransactionLastUpdated.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
            EffectiveDate = fundOrder.EffectiveDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            TransactionDateTime = fundOrder.TransactionDateTime.ToString(DateFormat, CultureInfo.InvariantCulture),
            SettlementDate = fundOrder.SettlementDate?.ToString(DateFormat, CultureInfo.InvariantCulture),
            OrderNo = fundOrder.OrderNo
        };
    }

    public static SiriusFundOrderHistoryResponse NewFundOrderHistoryResponse(FundOrder fundOrder)
    {
        return new SiriusFundOrderHistoryResponse
        {
            Unit = fundOrder.IsAllotted() ? decimal.Round(fundOrder.AllottedUnit ?? 0, 4) : decimal.Round(fundOrder.Unit ?? 0, 4),
            Amount = fundOrder.IsAllotted() ? decimal.Round(fundOrder.AllottedAmount ?? 0, 4) : decimal.Round(fundOrder.Amount ?? 0, 4),
            PaymentMethod = fundOrder.PaymentMethod,
            BankCode = fundOrder.BankCode ?? "",
            BankShortName = fundOrder.BankInfo?.ShortName!,
            BankAccount = fundOrder.BankAccount!,
            OrderId = fundOrder.BrokerOrderId,
            OrderNo = fundOrder.OrderNo,
            Account = fundOrder.AccountId,
            FundCode = fundOrder.FundCode,
            OrderType = fundOrder.OrderType.ToDescriptionString(),
            Status = NewSiriusFundStatus(fundOrder.Status),
            Nav = fundOrder.IsAllotted() ? decimal.Round(fundOrder.AllottedNav ?? 0, 4) :
                fundOrder.FundInfo?.Nav == null ? null : decimal.Round(fundOrder.FundInfo?.Nav ?? 0, 4),
            EffectiveDate = fundOrder.EffectiveDate.ToString(DateFormat, CultureInfo.InvariantCulture),
            TransactionDateTime = fundOrder.TransactionDateTime.ToString(DateFormat, CultureInfo.InvariantCulture),
            SettlementDate = fundOrder.SettlementDate?.ToString(DateFormat, CultureInfo.InvariantCulture),
            RejectReason = fundOrder.RejectReason,
            TransactionLastUpdated = fundOrder.TransactionLastUpdated.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
            UnitHolderId = fundOrder.UnitHolderId,
            Channel = fundOrder.Channel,
            AccountType = fundOrder.AccountType,
            PaymentStatus = fundOrder.PaymentStatus
        };
    }

    private static SiriusFundOrderStatus NewSiriusFundStatus(FundOrderStatus status)
    {
        return status switch
        {
            FundOrderStatus.Approved => SiriusFundOrderStatus.Processing,
            FundOrderStatus.Waiting => SiriusFundOrderStatus.Processing,
            FundOrderStatus.Rejected => SiriusFundOrderStatus.Rejected,
            FundOrderStatus.Cancelled => SiriusFundOrderStatus.Cancelled,
            FundOrderStatus.Expired => SiriusFundOrderStatus.Cancelled,
            FundOrderStatus.Allotted => SiriusFundOrderStatus.Matched,
            FundOrderStatus.Submitted => SiriusFundOrderStatus.Pending,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
