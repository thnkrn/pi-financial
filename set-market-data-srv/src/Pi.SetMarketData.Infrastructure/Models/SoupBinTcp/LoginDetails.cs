namespace Pi.SetMarketData.Infrastructure.Models.SoupBinTcp;

public class LoginDetails
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string RequestedSession { get; set; } = string.Empty;
    public ulong RequestedSequenceNumber { get; set; } = 0;
}