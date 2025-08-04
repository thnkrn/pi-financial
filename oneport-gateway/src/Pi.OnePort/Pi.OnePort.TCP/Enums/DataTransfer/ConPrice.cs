namespace Pi.OnePort.TCP.Enums.DataTransfer;

public enum ConPrice
{
    [SerializedValue(" ")]
    None,
    [SerializedValue("A")]
    Ato,
    [SerializedValue("C")]
    Atc,
    [SerializedValue("K")]
    Mkt,
    [SerializedValue("L")]
    Mtl,
}
