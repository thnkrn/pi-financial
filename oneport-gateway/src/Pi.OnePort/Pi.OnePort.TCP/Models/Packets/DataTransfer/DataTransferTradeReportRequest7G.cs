using Pi.OnePort.TCP.Enums.DataTransfer;

namespace Pi.OnePort.TCP.Models.Packets.DataTransfer;

public record DataTransferTradeReportRequest7G : IDataTransferPacketInBound
{
    public MessageType MessageType => MessageType.TradeReportRequest7G;

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}
