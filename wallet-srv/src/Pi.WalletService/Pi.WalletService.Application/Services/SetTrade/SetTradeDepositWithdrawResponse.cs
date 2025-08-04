namespace Pi.WalletService.Application.Services.SetTrade;

public record SetTradeDepositWithdrawResponse(bool Success, string UserId, string TransactionId,
    string SetTradeAccountNo, decimal Amount, string? ErrorCode = null, string? ErrorMessage = null);
