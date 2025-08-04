namespace Pi.BackofficeService.Application.Models.Sbl;

public record SblInstrument
{
    public required string Symbol { get; init; }
    public required decimal InterestRate { get; init; }
    public required decimal RetailLender { get; init; }
    public required decimal BorrowOutstanding { get; init; }
    public required decimal AvailableLending { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
