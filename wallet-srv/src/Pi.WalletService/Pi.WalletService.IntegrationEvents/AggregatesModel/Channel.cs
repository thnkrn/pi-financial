namespace Pi.WalletService.IntegrationEvents.AggregatesModel;

public enum Channel
{
    SetTrade,
    QR,
    ATS,
    OnlineViaKKP,
    EForm,
    TransferApp,
    // For Sirius as they might not sent one
    Unknown,
    ODD
}