namespace Pi.OnePort.TCP.Enums;

public enum RequestType
{
    [SerializedValue("LI")]
    Logon,

    [SerializedValue("LO")]
    Logout,

    [SerializedValue("TR")]
    Business,
}
