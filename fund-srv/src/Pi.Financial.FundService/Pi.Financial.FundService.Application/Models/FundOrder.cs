using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Models;

public class FundOrder
{
    public required decimal? Unit { get; init; }
    public required decimal? Amount { get; init; }
    public required decimal? AllottedNav { get; init; }
    public required decimal? AllottedUnit { get; init; }
    public required decimal? AllottedAmount { get; init; }
    public required string PaymentMethod { get; init; }
    public required string? BankAccount { get; init; }
    public required string? Edd { get; init; }
    public required string? SwitchIn { get; init; }
    public required string? SwitchTo { get; init; }
    public required string? BankCode { get; init; }
    public required string OrderNo { get; init; }
    public required string BrokerOrderId { get; init; }
    public required string UnitHolderId { get; init; }
    public required string? AmcOrderId { get; init; }
    public required string AccountId { get; init; }
    public required string FundCode { get; init; }
    public required bool? SellAllUnit { get; init; }
    public required string? CounterFundCode { get; init; }
    public required FundOrderType OrderType { get; init; }
    public required Channel Channel { get; init; }
    public required PaymentType? PaymentType { get; init; }
    public required FundAccountType? AccountType { get; init; }
    public required RedemptionType? RedemptionType { get; init; }
    public required FundOrderStatus Status { get; init; }
    public required OrderSide OrderSide { get; init; }
    public required DateTime TransactionLastUpdated { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required DateOnly? NavDate { get; init; }
    public required DateOnly? SettlementDate { get; init; }
    public required DateOnly? AllottedDate { get; init; }
    public required string? SaleLicense { get; init; }
    public required string? RejectReason { get; init; }
    public required string? SettlementBankAccount { get; init; }
    public required string? SettlementBankCode { get; init; }
    public required DateTime TransactionDateTime { get; init; }
    public string? CustCode { get; private set; }
    public BankInfo? BankInfo { get; private set; }
    public FundInfo? FundInfo { get; private set; }
    public string? PaymentStatus { get; init; }

    public void SetCustcode(string custCode)
    {
        CustCode = custCode;
    }

    public void SetBankInfo(BankInfo bankInfo)
    {
        BankInfo = bankInfo;
    }

    public void SetFundInfo(FundInfo fundInfo)
    {
        FundInfo = fundInfo;
    }

    public bool IsAllotted()
    {
        return Status == FundOrderStatus.Allotted;
    }
}
