using System.Text;

namespace Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

public class LoginAccepted : Message
{
    public LoginAccepted(string session, ulong sequenceNumber)
    {
        if (session.Length > 10) throw new ArgumentOutOfRangeException(session, "Session must have maximum length 10");

        const char type = 'A';
        var seqNo = sequenceNumber.ToString();
        seqNo = seqNo.PadLeft(20);
        var payload = type + session + seqNo;
        Bytes = Encoding.ASCII.GetBytes(payload);
    }

    internal LoginAccepted(byte[] bytes)
    {
        Bytes = bytes;
    }

    public string Session => Encoding.ASCII.GetString(Bytes.Skip(1).Take(10).ToArray());
    public ulong SequenceNumber => Convert.ToUInt64(Encoding.ASCII.GetString(Bytes.Skip(11).Take(20).ToArray()));
}