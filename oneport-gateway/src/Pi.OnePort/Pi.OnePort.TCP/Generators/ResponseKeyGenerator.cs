using Pi.OnePort.TCP.Constants;
using Pi.OnePort.TCP.Enums.DataTransfer;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Generators;

public interface IResponseKeyGenerator
{
    string? NewKey(Packet packet);
}

public class ResponseKeyGenerator : IResponseKeyGenerator
{
    public static readonly string DtKeyFormat = $"{{0,{FieldLength.MessageType}}}{{1,{FieldLength.RefOrderId}}}";

    public string? NewKey(Packet packet)
    {
        return packet.Data is not PacketDataTransfer data ? null : NewKey(data);
    }

    private static string? NewKey(PacketDataTransfer data)
    {
        string? result;
        switch (data.DataTransferPacketContent)
        {
            case DataTransferOrderAcknowledgementResponse7K response7K:
                MessageType? sourceType = response7K.ExecutionTransType switch
                {
                    ExecutionTransType.New => MessageType.NewOrderRequest7A,
                    ExecutionTransType.Cancel => MessageType.OrderCancelRequest7C,
                    _ => null,
                };
                result = sourceType != null ? string.Format(DtKeyFormat, sourceType.GetSerializedValue(), response7K.RefOrderId) : null;
                break;
            case DataTransferOrderChangeResponse7N response7N:
                result = string.Format(DtKeyFormat, MessageType.OrderChangeRequest7M.GetSerializedValue(), response7N.RefOrderId);
                break;
            case DataTransferOrderCancelRequest7C response7C:
                result = string.Format(DtKeyFormat, response7C.MessageType.GetSerializedValue(), response7C.RefOrderId);
                break;
            case DataTransferNewOrderRequest7A response7A:
                result = string.Format(DtKeyFormat, response7A.MessageType.GetSerializedValue(), response7A.RefOrderId);
                break;
            case DataTransferOrderChangeRequest7M response7M:
                result = string.Format(DtKeyFormat, response7M.MessageType.GetSerializedValue(), response7M.RefOrderId);
                break;
            default:
                result = null;
                break;
        }

        return result?.Replace(" ", "0");
    }
}
