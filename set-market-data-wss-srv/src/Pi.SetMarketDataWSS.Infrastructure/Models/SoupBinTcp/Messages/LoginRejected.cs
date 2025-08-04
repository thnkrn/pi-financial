using System.Text;

namespace Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

public class LoginRejected : Message
{
    public LoginRejected(char rejectReasonCode)
    {
        if (rejectReasonCode != 'A' && rejectReasonCode != 'S')
            throw new ArgumentException("Reject reason code must be either A or S", nameof(rejectReasonCode));

        const char type = 'J';
        var payload = new string(new[] { type, rejectReasonCode });
        Bytes = Encoding.ASCII.GetBytes(payload);
    }

    internal LoginRejected(byte[] bytes)
    {
        Bytes = bytes;
    }

    public char RejectReasonCode => Convert.ToChar(Bytes[1]);
}