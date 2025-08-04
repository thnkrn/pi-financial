namespace Pi.OnePort.TCP.Constants;

internal static class FieldLength
{
    public const int RefOrderId = 64;
    public const int FisOrderId = 10;
    public const int SecSymbol = 20;
    public const int Side = 1;
    public const int Type = 2;
    public const int MessageType = 2;
    public const int Volume = 8;
    public const int VolumeNew = 12;
    public const int ConfirmNo = 6;
    public const int SourceId = 1;
    public const int LastSequenceNumber = 8;
    public const int BeginSequenceNumber = 8;
    public const int EndSequenceNumber = 8;
    public const int HeartbeatInterval = 4;
    public const int Username = 20;
    public const int Password = 20;
    public const int HeartbeatReserveValue = 10;
    public const int HeartbeatResult = 1;
    public const int EnterId = 11;
    public const int Price = 13;
    public const int ConPrice = 1;
    public const int PublishVolume = 8;
    public const int Condition = 1;
    public const int Account = 10;
    public const int Ttf = 1;
    public const int OrderType = 1;
    public const int OrderStatus = 1;
    public const int CheckFlag = 8;
    public const int ExecutionTransType = 1;
    public const int ExecType = 1;
    public const int ExecutionTransRejectType = 2;
    public const int ReasonText = 128;
    public const int PortClient = 1;
    public const int MarketDealNo = 30;
    public const int MarketTradeNo = 30;
    public const int ValidTillData = 30;
}
