namespace Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;

public enum RejectionReason
{
    NotAuthorised = 1,
    SessionNotAvailable = 2
}

public class LoginStatus(bool success, RejectionReason rejectionReason = RejectionReason.NotAuthorised)
{
    public bool Success { get; } = success;
    public RejectionReason RejectionReason { get; } = rejectionReason;
}