using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public class UnitHolder : IAuditableEntity, IAggregateRoot
{
    public UnitHolder(Guid id, string tradingAccountNo, string amcCode, string unitHolderId, UnitHolderType unitHolderType, UnitHolderStatus status)
    {
        Id = id;
        TradingAccountNo = tradingAccountNo;
        AmcCode = amcCode;
        UnitHolderId = unitHolderId;
        UnitHolderType = unitHolderType;
        Status = status;
    }

    public Guid Id { get; set; }
    public string AmcCode { get; set; }
    public string UnitHolderId { get; set; }
    public string? CustomerCode { get; set; }
    public string TradingAccountNo { get; set; }
    public UnitHolderType UnitHolderType { get; set; }
    public UnitHolderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
