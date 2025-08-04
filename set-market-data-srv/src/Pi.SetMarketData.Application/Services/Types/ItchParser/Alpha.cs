using System.Text;

namespace Pi.SetMarketData.Application.Services.Types.ItchParser;

public class Alpha
{
    public string Value { get; set; }

    public Alpha(ReadOnlySpan<byte> inputData, int fieldLength)
    {
        // Decode the byte array using ISO 8859-1 encoding
        string decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(inputData);

        if (decodedString.Length > fieldLength)
        {
            Value = decodedString.Substring(0, fieldLength);
        }
        else
        {
            Value = decodedString.PadRight(fieldLength, ' ');
        }
    }

    public static implicit operator string(Alpha alpha) => alpha.Value;

    public override string ToString() => Value.ToString();
}
