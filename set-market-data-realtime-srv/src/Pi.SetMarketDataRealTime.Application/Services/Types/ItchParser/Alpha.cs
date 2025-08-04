using System.Runtime.Serialization;
using System.Text;

namespace Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

[Serializable]
public class Alpha : ISerializable, IEquatable<Alpha>
{
    public Alpha(ReadOnlySpan<byte> inputData, int fieldLength)
    {
        if (inputData == null)
            throw new ArgumentNullException(nameof(inputData));
        if (fieldLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(fieldLength), "Field length must be positive.");

        var decodedString = Encoding.GetEncoding("ISO-8859-1").GetString(inputData);
        Value = decodedString.Length > fieldLength
            ? decodedString.Substring(0, fieldLength)
            : decodedString.PadRight(fieldLength, ' ');
    }

    public Alpha(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected Alpha(SerializationInfo info, StreamingContext context)
    {
        Value = info.GetString("Value") ?? throw new SerializationException("Value cannot be null.");
    }

    public string Value { get; }

    public bool Equals(Alpha? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Value", Value);
    }

    public static implicit operator string(Alpha alpha)
    {
        return alpha.Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Alpha)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Alpha? left, Alpha? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Alpha? left, Alpha? right)
    {
        return !Equals(left, right);
    }
}