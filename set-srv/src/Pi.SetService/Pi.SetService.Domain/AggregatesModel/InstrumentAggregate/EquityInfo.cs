using Pi.Common.SeedWork;

namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public class EquityInfo(Guid id, string symbol, string marginCode, bool isTurnoverList)
    : IAggregateRoot
{
    public Guid Id { get; init; } = id;
    public string Symbol { get; init; } = symbol;
    public string MarginCode { get; set; } = marginCode;
    public bool IsTurnoverList { get; set; } = isTurnoverList;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime RowVersion { get; init; }
}