namespace Pi.OnePort.TCP.Constants;

public abstract class DataFormat
{
    public const string Date = "yyyyMMdd";
    public const string DateTime = "yyyyMMdd-HHmmss";
    public static readonly string Etx = $"{(char)3}";
}
