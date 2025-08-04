// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Factories;

public static class FundTradingFactory
{
    public static FundOrder NewFundOrder(FundOrderState src)
    {
        return new FundOrder
        {
            Unit = src.Unit,
            Amount = src.Amount,
            AllottedNav = src.AllottedNav,
            AllottedUnit = src.AllottedUnit,
            AllottedAmount = src.AllottedAmount,
            PaymentMethod = src.PaymentType?.ToDescriptionString() ?? "",
            BankAccount = src.BankAccount,
            Edd = null,
            SwitchIn = null,
            SwitchTo = null,
            BankCode = src.BankCode,
            OrderNo = src.OrderNo ?? "",
            BrokerOrderId = src.BrokerOrderId ?? "",
            UnitHolderId = src.UnitHolderId ?? "",
            AmcOrderId = src.AmcOrderId,
            AccountId = src.TradingAccountNo,
            FundCode = src.FundCode,
            SellAllUnit = src.SellAllUnit,
            CounterFundCode = src.CounterFundCode,
            OrderType = src.OrderType,
            Channel = src.Channel,
            PaymentType = src.PaymentType,
            AccountType = src.AccountType,
            RedemptionType = src.RedemptionType,
            Status = src.OrderStatus,
            OrderSide = src.OrderSide,
            TransactionLastUpdated = src.UpdatedAt ?? DateTime.MinValue,
            EffectiveDate = src.EffectiveDate ?? DateOnly.MinValue,
            NavDate = src.NavDate,
            SettlementDate = src.SettlementDate,
            AllottedDate = src.AllottedDate,
            SaleLicense = src.SaleLicense,
            RejectReason = string.IsNullOrEmpty(src.RejectReason) ? src.FailedReason : src.RejectReason,
            SettlementBankAccount = src.SettlementBankAccount,
            SettlementBankCode = src.SettlementBankCode,
            TransactionDateTime = src.CreatedAt,
        };
    }
}
