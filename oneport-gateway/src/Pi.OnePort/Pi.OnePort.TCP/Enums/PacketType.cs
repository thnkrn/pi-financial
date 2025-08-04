namespace Pi.OnePort.TCP.Enums;

public enum PacketType
{
    [SerializedValue("LI")]
    Logon,

    [SerializedValue("LO")]
    Logout,

    [SerializedValue("HB")]
    Heartbeat,

    [SerializedValue("TR")]
    TestRequest,

    [SerializedValue("RR")]
    RecoveryRequest,

    [SerializedValue("RA")]
    RecoveryAcknowledge,

    [SerializedValue("RC")]
    RecoveryComplete,

    [SerializedValue("DT")]
    DataTransfer,
}
