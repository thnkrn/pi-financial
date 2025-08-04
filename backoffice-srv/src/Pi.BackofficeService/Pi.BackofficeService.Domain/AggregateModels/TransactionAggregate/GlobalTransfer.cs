namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public class GlobalTransfer
{
    public GlobalTransfer(string globalAccount)
    {
        GlobalAccount = globalAccount;
    }

    public string GlobalAccount { get; set; }
    public decimal? RequestedFxRate { get; set; }
    public Currency? RequestedFxCurrency { get; set; }
    public decimal? FxRate { get; set; }
    public DateTime? FxRequestDateTime { get; set; }
    public DateTime? FxConfirmDateTime { get; set; }
    public string? TransferFromAccount { get; set; }
    public string? TransferToAccount { get; set; }
    public decimal? TransferAmount { get; set; }
    public decimal? TransactionFee { get; set; }
    public DateTime? TransferRequestDateTime { get; set; }
    public DateTime? TransferConfirmDateTime { get; set; }
    public decimal? RefundAmountThb { get; set; }
    public decimal? NetAmountThb { get; set; }
}
