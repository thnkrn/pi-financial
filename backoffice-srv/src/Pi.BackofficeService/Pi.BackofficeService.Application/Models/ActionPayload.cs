namespace Pi.BackofficeService.Application.Models;

public record ConfirmKkpActionPayload
{
    public decimal PaymentReceivedAmount { get; set; }
    public DateTime PaymentReceivedDateTime { get; set; }
    public string BankCode { get; set; } = string.Empty;
    public string BankAccountNo { get; set; } = string.Empty;
}