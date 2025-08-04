using Pi.Common.SeedWork;

namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public class SblInstrument : IAuditableEntity
{
    public SblInstrument(Guid id, string symbol, decimal interestRate, decimal retailLender, decimal borrowOutstanding, decimal availableLending)
    {
        Id = id;
        Symbol = symbol;
        InterestRate = interestRate;
        RetailLender = retailLender;
        BorrowOutstanding = borrowOutstanding;
        AvailableLending = availableLending;
    }

    public SblInstrument(Guid id, string symbol, decimal interestRate, decimal retailLender)
    {
        Id = id;
        Symbol = symbol;
        InterestRate = interestRate;
        RetailLender = retailLender;
        BorrowOutstanding = 0;
        AvailableLending = retailLender;
    }

    public Guid Id { get; init; }
    public string Symbol { get; init; }
    public decimal InterestRate { get; init; }
    public decimal RetailLender { get; init; }
    public decimal BorrowOutstanding { get; private set; }
    public decimal AvailableLending { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Borrow(int volume)
    {
        AvailableLending -= volume;
        BorrowOutstanding += volume;
    }

    public void Return(int volume)
    {
        AvailableLending += volume;
        BorrowOutstanding -= volume;
    }
}
