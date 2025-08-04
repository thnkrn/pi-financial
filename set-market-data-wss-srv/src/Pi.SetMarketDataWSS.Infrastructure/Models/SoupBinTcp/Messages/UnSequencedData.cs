namespace Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

public class UnSequencedData : Message
{
    public UnSequencedData(byte[] message)
    {
        const char type = 'U';
        var messageLength = message.Length;
        var payload = new byte[messageLength + 1];
        payload[0] = Convert.ToByte(type);
        Array.Copy(message, 0, payload, 1, messageLength);
        Bytes = payload;
    }

    internal UnSequencedData(byte[] bytes, bool addToBytesDirectly)
    {
        if (addToBytesDirectly) Bytes = bytes;
    }

    public byte[] Message => Bytes.Skip(1).Take(Length - 1).ToArray();
}