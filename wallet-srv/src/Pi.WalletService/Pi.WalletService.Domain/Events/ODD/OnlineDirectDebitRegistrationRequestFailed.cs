namespace Pi.WalletService.Domain.Events.ODD;

public record OnlineDirectDebitRegistrationRequestFailed(Guid UserId, string ErrorCode);