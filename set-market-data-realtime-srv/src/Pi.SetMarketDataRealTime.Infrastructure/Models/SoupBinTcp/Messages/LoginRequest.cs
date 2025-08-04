using System.Text;

namespace Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

public class LoginRequest : Message
{
    public LoginRequest(string? username, string password, string requestedSession = "",
        ulong requestedSequenceNumber = 0)
    {
        const char type = 'L';
        var seqNo = requestedSequenceNumber.ToString().PadLeft(20);
        username = username?.PadRight(6);
        password = password.PadRight(10);
        requestedSession = requestedSession.PadLeft(10);
        var payload = type + username + password + requestedSession + seqNo;

        Bytes = Encoding.ASCII.GetBytes(payload);
    }

    internal LoginRequest(byte[]? bytes)
    {
        Bytes = bytes;
    }

    public string Username => Bytes != null ? Encoding.ASCII.GetString(Bytes.Skip(1).Take(6).ToArray()) : string.Empty;

    public string Password => Bytes != null ? Encoding.ASCII.GetString(Bytes.Skip(7).Take(10).ToArray()) : string.Empty;

    public string RequestedSession =>
        Bytes != null ? Encoding.ASCII.GetString(Bytes.Skip(17).Take(10).ToArray()) : string.Empty;

    public ulong RequestedSequenceNumber =>
        Bytes != null ? Convert.ToUInt64(Encoding.ASCII.GetString(Bytes.Skip(27).Take(20).ToArray())) : 0;
}