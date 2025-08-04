using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using OrderState = Pi.Financial.FundService.IntegrationEvents.Models.FundOrderState;

namespace Pi.Financial.FundService.Application.Factories;

public static class EntityFactory
{
    public static FundOrderState NewFundOrderState(FundOrder fundOrder, Guid? tradingAccountId)
    {

        return new FundOrderState(Guid.NewGuid(), fundOrder.AccountId, fundOrder.FundCode, fundOrder.Channel)
        {
            TradingAccountId = tradingAccountId,
            CustomerCode = fundOrder.CustCode,
            OrderNo = fundOrder.OrderNo,
            OrderType = fundOrder.OrderType,
            CurrentState = OrderState.GetName(() => OrderState.OrderPlaced),
            BrokerOrderId = fundOrder.BrokerOrderId,
            AmcOrderId = fundOrder.AmcOrderId,
            UnitHolderId = fundOrder.UnitHolderId,
            CounterFundCode = fundOrder.CounterFundCode,
            AllottedAmount = fundOrder.AllottedAmount,
            AllottedUnit = fundOrder.AllottedUnit,
            AllottedNav = fundOrder.AllottedNav,
            NavDate = fundOrder.NavDate,
            AllottedDate = fundOrder.AllottedDate,
            RedemptionType = fundOrder.RedemptionType,
            BankAccount = fundOrder.BankAccount?.Trim() != "" ? fundOrder.BankAccount?.Trim() : null,
            AccountType = fundOrder.AccountType,
            SellAllUnit = fundOrder.SellAllUnit,
            PaymentType = fundOrder.PaymentType,
            Unit = fundOrder.Unit,
            Amount = fundOrder.Amount,
            BankCode = fundOrder.BankCode?.Trim() != "" ? fundOrder.BankCode?.Trim() : null,
            SettlementBankAccount = fundOrder.SettlementBankAccount?.Trim() != "" ? fundOrder.SettlementBankAccount?.Trim() : null,
            SettlementBankCode = fundOrder.SettlementBankCode?.Trim() != "" ? fundOrder.SettlementBankCode?.Trim() : null,
            EffectiveDate = fundOrder.EffectiveDate,
            SettlementDate = fundOrder.SettlementDate,
            RejectReason = fundOrder.RejectReason,
            OrderSide = fundOrder.OrderSide,
            SaleLicense = fundOrder.SaleLicense,
            OrderStatus = fundOrder.Status,
        };
    }

    public static FundOrderState NewFundOrderState(FundOrderRequestReceived fundOrderRequest)
    {
        return new FundOrderState(fundOrderRequest.CorrelationId, fundOrderRequest.TradingAccountNo, fundOrderRequest.FundCode, fundOrderRequest.Channel)
        {
            TradingAccountId = fundOrderRequest.TradingAccountId,
            CorrelationId = fundOrderRequest.CorrelationId,
            FundCode = fundOrderRequest.FundCode,
            TradingAccountNo = fundOrderRequest.TradingAccountNo,
            EffectiveDate = fundOrderRequest.EffectiveDate,
            CustomerCode = fundOrderRequest.CustomerCode
        };
    }

    public static void SyncFundOrderState(FundOrderState orderState, FundOrder fundOrder)
    {
        orderState.OrderStatus = fundOrder.Status;
        orderState.AccountType = fundOrder.AccountType;
        orderState.SettlementDate = fundOrder.SettlementDate;
        orderState.NavDate = fundOrder.NavDate;
        orderState.BrokerOrderId = fundOrder.BrokerOrderId;
        orderState.SettlementBankAccount = fundOrder.SettlementBankAccount?.Trim() != ""
            ? fundOrder.SettlementBankAccount?.Trim()
            : null;
        orderState.SettlementBankCode =
            fundOrder.SettlementBankCode?.Trim() != "" ? fundOrder.SettlementBankCode?.Trim() : null;
        orderState.AmcOrderId = fundOrder.AmcOrderId;
        orderState.RejectReason = fundOrder.RejectReason;
        orderState.AllottedAmount = fundOrder.AllottedAmount;
        orderState.AllottedDate = fundOrder.AllottedDate;
        orderState.AllottedNav = fundOrder.AllottedNav;
        orderState.AllottedUnit = fundOrder.AllottedUnit;
    }
}
