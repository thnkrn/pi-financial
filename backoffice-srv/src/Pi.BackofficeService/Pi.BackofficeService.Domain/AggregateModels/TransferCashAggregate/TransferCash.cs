using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;

public class TransferCash
{
    public Guid Id { get; set; }
    public string? State { get; set; }
    public string? TransactionNo { get; set; }
    public string? Status { get; set; }
    public string? CustomerName { get; set; }
    public string? TransferFromAccountCode { get; set; }
    public string? TransferToAccountCode { get; set; }
    public Product? TransferFromExchangeMarket { get; set; }
    public Product? TransferToExchangeMarket { get; set; }
    public decimal? Amount { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public DateTime? CreatedAt { get; set; }
}