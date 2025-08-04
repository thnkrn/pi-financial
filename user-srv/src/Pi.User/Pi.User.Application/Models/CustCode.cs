namespace Pi.User.Application.Models;

public record CustomerCode(string Code, List<string> TradingAccounts);

public record CustomerCodeHasPin(string CustomerCode, bool HasPin);
