namespace Pi.OnePort.TCP.Enums;

[AttributeUsage(AttributeTargets.Field)]
public class SerializedValue : Attribute
{
    public string Value { get; }

    public SerializedValue(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
