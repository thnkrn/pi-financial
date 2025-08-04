namespace Pi.WalletService.IntegrationEvents
{
    public record OnlineDirectDebitRegistrationFailedEvent(string RefCode, Guid UserId, string BankAccountNo, string OddBank);
}
