using Pi.Common.SeedWork;

namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public class EquityInitialMargin(Guid id, string marginCode, decimal rate) : IAggregateRoot
{
    public Guid Id { get; private set; } = id;
    public string MarginCode { get; private set; } = marginCode;
    public decimal Rate { get; set; } = rate;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime RowVersion { get; set; }
}
