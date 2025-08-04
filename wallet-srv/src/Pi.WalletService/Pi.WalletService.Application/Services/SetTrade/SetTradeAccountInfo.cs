using System.Text.Json.Serialization;

namespace Pi.WalletService.Application.Services.SetTrade;

public record SetTradeAccountInfo(
        [property: JsonPropertyName("creditLine")]
        decimal CreditLine,
        [property: JsonPropertyName("excessEquity")]
        decimal ExcessEquity,
        [property: JsonPropertyName("cashBalance")]
        decimal CashBalance,
        [property: JsonPropertyName("equity")]
        decimal Equity,
        [property: JsonPropertyName("liquidationValue")]
        decimal LiquidationValue,
        [property: JsonPropertyName("depositWithdrawal")]
        decimal DepositWithdrawal
    );
