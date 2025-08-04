namespace Pi.User.IntegrationEvents;

public record AddBankAccountWithEffectiveEvent(
  Guid UserId,
  string ReferId,
  string TransId,
  string CustomerCode,
  string BankAccountNo,
  string BankCode
);
