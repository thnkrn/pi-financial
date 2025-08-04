namespace Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

public abstract class Message
{
    private const int LengthOfLengthField = 2;

    public byte[] Bytes { get; protected set; }

    public ushort Length => (ushort)Bytes.Length;

    public byte[] TotalBytes
    {
        get
        {
            var result = new byte[Bytes.Length + LengthOfLengthField];
            var lengthBytes = BitConverter.GetBytes(Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);

            Array.Copy(lengthBytes, 0, result, 0, lengthBytes.Length);
            Array.Copy(Bytes, 0, result, LengthOfLengthField, Bytes.Length);
            return result;
        }
    }

    public char Type => Convert.ToChar(Bytes[0]);
}