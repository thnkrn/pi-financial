using System.Text;

namespace Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

public class Alpha
{
    public Alpha(ReadOnlySpan<byte> inputData, int fieldLength)
    {
        // Decode the byte array using ISO 8859-1 encoding
        var decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(inputData);

        if (decodedString.Length > fieldLength)
            Value = decodedString.Substring(0, fieldLength);
        else
            Value = decodedString.PadRight(fieldLength, ' ');
    }
    

    public string Value { get; set; }

    public static implicit operator string(Alpha alpha)
    {
        return alpha.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}