using System;

namespace Pi.WalletService.IntegrationEvents
{
    public record OnlineDirectDebitRegistrationSuccessEvent(string RefCode, Guid UserId, string BankAccountNo, string OddBank);
}
