namespace Pi.User.IntegrationEvents;

public record UpdateBankAccountEffectiveDateFailedEvent(string CustomerCode, string ErrorCode);
