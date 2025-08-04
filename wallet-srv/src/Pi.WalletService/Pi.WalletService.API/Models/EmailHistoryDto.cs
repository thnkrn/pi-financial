namespace Pi.WalletService.API.Models;

public class EmailHistoryDto
{
    public EmailHistoryDto(string transactionNo, string emailType, DateTime sentAt)
    {
        TransactionNo = transactionNo;
        EmailType = emailType;
        SentAt = sentAt;
    }

    public string TransactionNo { get; set; }
    public string EmailType { get; set; }
    public DateTime SentAt { get; set; }

}