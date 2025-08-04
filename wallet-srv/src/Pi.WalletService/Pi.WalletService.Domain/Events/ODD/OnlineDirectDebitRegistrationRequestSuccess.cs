namespace Pi.WalletService.Domain.Events.ODD;

public record OnlineDirectDebitRegistrationRequestSuccess(Guid UserId, string WebUrl);