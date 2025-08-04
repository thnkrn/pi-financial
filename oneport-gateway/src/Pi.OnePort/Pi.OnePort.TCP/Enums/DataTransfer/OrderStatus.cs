namespace Pi.OnePort.TCP.Enums.DataTransfer;

public enum OrderStatus
{
    [SerializedValue("0")]
    Accepted,

    [SerializedValue("7")]
    Warning,

    [SerializedValue("8")]
    Rejected,
}
