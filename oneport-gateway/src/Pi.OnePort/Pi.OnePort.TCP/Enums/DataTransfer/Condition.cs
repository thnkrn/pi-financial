namespace Pi.OnePort.TCP.Enums.DataTransfer;

public enum Conditions
{
    [SerializedValue(" ")]
    None,
    [SerializedValue("I")]
    Ioc,
    [SerializedValue("F")]
    Fok,
    [SerializedValue("O")]
    Odd,
    [SerializedValue("C")]
    Gtc,
    [SerializedValue("D")]
    Gtd,
}
