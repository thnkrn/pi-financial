namespace Pi.WalletService.API.Models;

public enum DepositCashChannel
{
    SetTrade,
    QR,
    ATS,
    OnlineViaKKP,
    ODD,
}
public record DepositCashRequest(
    string TransactionId,
    decimal Amount,
    string CustomerCode,
    string TradingAccountCode,
    string BankCode,
    DepositCashChannel Channel
);