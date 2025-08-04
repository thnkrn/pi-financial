namespace Pi.OnePort.TCP.Enums.DataTransfer;

public enum ExecutionTransType
{
    [SerializedValue("0")]
    New,

    [SerializedValue("1")]
    Cancel,

    [SerializedValue("2")]
    ChangeAcct,

    [SerializedValue("3")]
    Reject,
}
