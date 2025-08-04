using System.Globalization;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.Services;
using static Pi.Financial.Client.FundConnext.Model.FundOrder;
using FundOrder = Pi.Financial.FundService.Application.Models.FundOrder;

namespace Pi.Financial.FundService.Infrastructure.Factories;

public static class ApplicationFactory
{
    public static CustomerAccountDetail NewCustomerAccount(AccountV5Response account,
        IndividualInvestorV5Response.InvestorClassEnum? responseInvestorClass = null,
        IndividualInvestorV5Response.InvestorTypeEnum? responseInvestorType = null,
        string? juristicNumber = null)
    {
        return new CustomerAccountDetail
        {
            IcLicense = account.IcLicense,
            InvestorClass = NewInvestorClass(responseInvestorClass),
            CustomerAccountUnitHolders = account.Unitholders.Select(NewCustomerAccountUnitHolder).ToList(),
            InvestorType = NewInvestorType(responseInvestorType),
            JuristicNumber = juristicNumber
        };
    }

    private static CustomerAccountUnitHolder NewCustomerAccountUnitHolder(UnitholderResponse unitHolder)
    {
        return new CustomerAccountUnitHolder
        {
            AmcCode = unitHolder.AmcCode,
            TradingAccountNo = unitHolder.AccountId,
            UnitHolderId = unitHolder.UnitholderId,
            Status = NewUnitHolderStatus(unitHolder.Status),
            UnitHolderType = NewUnitHolderType(unitHolder.UnitholderType)
        };
    }

    public static FundOrder NewFundOrder(Client.FundConnext.Model.FundOrder fundOrder)
    {
        CultureInfo provider = CultureInfo.InvariantCulture;
        var paymentMethod = fundOrder.PaymentType;
        if (fundOrder.PaymentType != null && fundOrder.PaymentType.EndsWith("_SA"))
        {
            // Remove Suffix "_SA"
            paymentMethod = fundOrder.PaymentType.Substring(0, fundOrder.PaymentType.Length - "_SA".Length);
        }

        return new FundOrder
        {
            Unit = fundOrder.Unit,
            Amount = fundOrder.Amount,
            PaymentMethod = paymentMethod,
            SellAllUnit = fundOrder.SellAllUnitFlag == SellAllUnitFlagEnum.Y,
            SaleLicense = fundOrder.IcLicense,
            BankAccount = fundOrder.BankAccount != "" ? fundOrder.BankAccount : null,
            AmcOrderId = fundOrder.AmcOrderReferenceNo,
            AllottedNav = fundOrder.AllottedNAV,
            AllottedAmount = fundOrder.AllottedAmount,
            AllottedUnit = fundOrder.AllottedUnit,
            CounterFundCode = fundOrder.CounterFundCode,
            RejectReason = fundOrder.RejectReason,
            Edd = null,
            SwitchIn = null,
            SwitchTo = null,
            BankCode = fundOrder.BankCode != "" ? fundOrder.BankCode : null,
            SettlementBankAccount = fundOrder.SettlementBankAccount != "" ? fundOrder.SettlementBankAccount : null,
            SettlementBankCode = fundOrder.SettlementBankCode != "" ? fundOrder.SettlementBankCode : null,
            OrderNo = fundOrder.SaOrderReferenceNo,
            BrokerOrderId = fundOrder.TransactionId,
            UnitHolderId = fundOrder.UnitholderId,
            Channel = NewChannelType(fundOrder.Channel),
            AccountId = fundOrder.AccountId,
            FundCode = fundOrder.FundCode,
            PaymentType = fundOrder.PaymentType != null ? NewPaymentType(fundOrder.PaymentType) : null,
            OrderSide = NewOrderSide(fundOrder.OrderType),
            AccountType = fundOrder.AccountType != null ? NewAccountType(fundOrder.AccountType) : null,
            OrderType = NewFundOrderType(fundOrder.OrderType),
            RedemptionType = fundOrder.RedemptionType != null ? NewRedemptionType(fundOrder.RedemptionType) : null,
            Status = NewFundStatus(fundOrder.Status),
            TransactionLastUpdated = DateTime.ParseExact(fundOrder.TransactionLastUpdated,
                FundConnextService.DateTimeFormatPattern,
                provider),
            EffectiveDate = DateOnly.ParseExact(fundOrder.EffectiveDate,
                FundConnextService.DateFormatPattern,
                provider),
            TransactionDateTime = DateTime.ParseExact(fundOrder.TransactionDateTime,
                FundConnextService.DateTimeFormatPattern,
                provider),
            NavDate = fundOrder.NavDate != null ? DateOnly.ParseExact(fundOrder.NavDate,
                FundConnextService.DateFormatPattern,
                provider) : null,
            AllottedDate = fundOrder.AllotmentDate != null ? DateOnly.ParseExact(fundOrder.AllotmentDate,
                FundConnextService.DateFormatPattern,
                provider) : null,
            SettlementDate = fundOrder.SettlementDate != null ? DateOnly.ParseExact(fundOrder.SettlementDate,
                FundConnextService.DateFormatPattern,
                provider) : null,
            PaymentStatus = fundOrder.PaymentStatus
        };
    }

    private static InvestorClass? NewInvestorClass(IndividualInvestorV5Response.InvestorClassEnum? investorClassEnum)
    {
        return investorClassEnum switch
        {
            IndividualInvestorV5Response.InvestorClassEnum._1 => InvestorClass.UltraHighNetWorth,
            IndividualInvestorV5Response.InvestorClassEnum._2 => InvestorClass.HighNetWorth,
            IndividualInvestorV5Response.InvestorClassEnum._3 => InvestorClass.Retail,
            IndividualInvestorV5Response.InvestorClassEnum._4 => InvestorClass.Institutional,
            _ => null
        };
    }

    private static InvestorType? NewInvestorType(IndividualInvestorV5Response.InvestorTypeEnum? investorTypeEnum)
    {
        return investorTypeEnum switch
        {
            IndividualInvestorV5Response.InvestorTypeEnum.INDIVIDUAL => InvestorType.INDIVIDUAL,
            IndividualInvestorV5Response.InvestorTypeEnum.JOINT => InvestorType.JOINT,
            IndividualInvestorV5Response.InvestorTypeEnum.BYWHOM => InvestorType.BYWHOM,
            IndividualInvestorV5Response.InvestorTypeEnum.FORWHOM => InvestorType.FORWHOM,
            IndividualInvestorV5Response.InvestorTypeEnum.JURISTIC => InvestorType.JURISTIC,
            _ => null
        };
    }

    private static FundOrderType NewFundOrderType(string orderType)
    {
        return orderType switch
        {
            "SUB" => FundOrderType.Subscription,
            "RED" => FundOrderType.Redemption,
            "SWO" => FundOrderType.SwitchOut,
            "SWI" => FundOrderType.SwitchIn,
            "XSO" => FundOrderType.CrossSwitchOut,
            "XSI" => FundOrderType.CrossSwitchIn,
            "TRO" => FundOrderType.TransferOut,
            "TRI" => FundOrderType.TransferIn,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    private static OrderSide NewOrderSide(string orderType)
    {
        return orderType switch
        {
            "SUB" => OrderSide.Buy,
            "RED" => OrderSide.Sell,
            "SWO" => OrderSide.Switch,
            "SWI" => OrderSide.Switch,
            "XSO" => OrderSide.CrossSwitch,
            "XSI" => OrderSide.CrossSwitch,
            "TRO" => OrderSide.Transfer,
            "TRI" => OrderSide.Transfer,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    private static UnitHolderStatus NewUnitHolderStatus(Pi.Financial.Client.FundConnext.Model.UnitholderResponse.StatusEnum? unitHolderStatus)
    {
        return unitHolderStatus switch
        {
            Client.FundConnext.Model.UnitholderResponse.StatusEnum.ACTIVE => UnitHolderStatus.Normal,
            Client.FundConnext.Model.UnitholderResponse.StatusEnum.CLOSED => UnitHolderStatus.Disabled,
            Client.FundConnext.Model.UnitholderResponse.StatusEnum.SUSPENDED => UnitHolderStatus.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(UnitHolderStatus), unitHolderStatus, null)
        };
    }

    private static PaymentType NewPaymentType(string paymentType)
    {
        return paymentType switch
        {
            "ATS_SA" => PaymentType.AtsSa,
            "EATS_SA" => PaymentType.EatsSa,
            "TRN_SA" => PaymentType.TrnSa,
            "CHQ_SA" => PaymentType.ChqSa,
            "CRC_SA" => PaymentType.CrcSa,
            "COL_SA" => PaymentType.ColSa,
            "ATS_AMC" => PaymentType.AtsAmc,
            "TRN_AMC" => PaymentType.TrnAmc,
            "CHQ_AMC" => PaymentType.ChqAmc,
            "CRC_AMC" => PaymentType.CrcAmc,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType), paymentType, null)
        };
    }

    private static RedemptionType NewRedemptionType(string redemptionType)
    {
        return redemptionType switch
        {
            "UNIT" => RedemptionType.Unit,
            "AMT" => RedemptionType.Amount,
            _ => throw new ArgumentOutOfRangeException(nameof(redemptionType), redemptionType, null)
        };
    }

    private static Channel NewChannelType(string channel)
    {
        return channel switch
        {
            "ONL" => Channel.ONL,
            "MKT" => Channel.MKT,
            "MOB" => Channel.MOB,
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }

    private static FundAccountType NewAccountType(string accountType)
    {
        return accountType switch
        {
            "SEG" => FundAccountType.SEG,
            "OMN" => FundAccountType.OMN,
            "SEG_NT" => FundAccountType.SEG_NT,
            "SEG_T" => FundAccountType.SEG_T,
            "OMN_NT" => FundAccountType.OMN_NT,
            "OMT_T" => FundAccountType.OMN_T,
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null)
        };
    }

    private static UnitHolderType NewUnitHolderType(string unitHolderType)
    {
        return unitHolderType switch
        {
            "SEG" => UnitHolderType.SEG,
            "OMN" => UnitHolderType.OMN,
            _ => throw new ArgumentOutOfRangeException(nameof(unitHolderType), unitHolderType, null)
        };
    }

    private static FundOrderStatus NewFundStatus(StatusEnum status)
    {
        return status switch
        {
            StatusEnum.APPROVED => FundOrderStatus.Approved,
            StatusEnum.WAITING => FundOrderStatus.Waiting,
            StatusEnum.REJECTED => FundOrderStatus.Rejected,
            StatusEnum.CANCELLED => FundOrderStatus.Cancelled,
            StatusEnum.EXPIRED => FundOrderStatus.Expired,
            StatusEnum.ALLOTTED => FundOrderStatus.Allotted,
            StatusEnum.SUBMITTED => FundOrderStatus.Submitted,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
