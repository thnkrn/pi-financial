using System.ComponentModel;

namespace Pi.WalletService.Application.Services.Bank;

public enum OnlineDirectDebitBank
{
    [Description("Scb")]
    Scb = 1,
    [Description("Kbank")]
    Kbank = 2
}