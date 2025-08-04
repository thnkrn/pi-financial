using Microsoft.Extensions.Logging;
using Pi.OnePort.TCP.Client;
using Pi.OnePort.TCP.Exceptions;
using Pi.OnePort.TCP.Generators;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.Api;

public interface IOnePortApi
{
    Task<DataTransferOrderAcknowledgementResponse7K> NewOrderAsync(DataTransferNewOrderRequest7A request, CancellationToken ct = default);
    Task<DataTransferOrderChangeResponse7N> ChangeOrderAsync(DataTransferOrderChangeRequest7M request, CancellationToken ct = default);
    Task<DataTransferOrderAcknowledgementResponse7K> CancelOrderAsync(DataTransferOrderCancelRequest7C request, CancellationToken ct = default);
}

public class OnePortApi : IOnePortApi
{
    private readonly IOnePortResponseMapClient _onePortClient;
    private readonly IResponseKeyGenerator _keyGenerator;
    private readonly ILogger<OnePortApi> _logger;
    private int _sequence;

    public OnePortApi(IOnePortResponseMapClient onePortClient, IResponseKeyGenerator keyGenerator, ILogger<OnePortApi> logger)
    {
        _onePortClient = onePortClient;
        _keyGenerator = keyGenerator;
        _logger = logger;
    }

    public async Task<DataTransferOrderAcknowledgementResponse7K> NewOrderAsync(DataTransferNewOrderRequest7A request, CancellationToken ct = default)
    {
        var response = await Send(new PacketDataTransfer(request), ct);
        if (response == null)
        {
            _logger.LogError("Can't get response for NewOrderAsync");
            throw new OnePortApiException("NewOrder failed");
        }

        if (response.DataTransferPacketContent is not DataTransferOrderAcknowledgementResponse7K data)
        {
            _logger.LogError("NewOrderAsync received unexpected type: {Type}", response.DataTransferPacketContent.GetType());
            throw new OnePortApiException($"NewOrder received unexpected type: {response.DataTransferPacketContent.GetType()}");
        }

        return data;
    }

    public async Task<DataTransferOrderChangeResponse7N> ChangeOrderAsync(DataTransferOrderChangeRequest7M request, CancellationToken ct = default)
    {
        var response = await Send(new PacketDataTransfer(request), ct);

        if (response == null)
        {
            _logger.LogError("Can't get response for ChangeOrderAsync");
            throw new OnePortApiException("Change order failed");
        }

        if (response.DataTransferPacketContent is not DataTransferOrderChangeResponse7N data)
        {
            _logger.LogError("ChangeOrderAsync received unexpected type: {Type}", response.DataTransferPacketContent.GetType());
            throw new OnePortApiException($"ChangeOrder received unexpected type: {response.DataTransferPacketContent.GetType()}");
        }

        return data;
    }

    public async Task<DataTransferOrderAcknowledgementResponse7K> CancelOrderAsync(DataTransferOrderCancelRequest7C request, CancellationToken ct = default)
    {
        var response = await Send(new PacketDataTransfer(request), ct);

        if (response == null)
        {
            _logger.LogError("Can't get response for CancelOrderAsync");
            throw new OnePortApiException("Cancel order failed");
        }

        if (response.DataTransferPacketContent is not DataTransferOrderAcknowledgementResponse7K data)
        {
            _logger.LogError("CancelOrder received unexpected type: {Type}", response.DataTransferPacketContent.GetType());
            throw new OnePortApiException(
                $"CancelOrder received unexpected type: {response.DataTransferPacketContent.GetType()}");
        }

        return data;
    }

    private async Task<PacketDataTransfer?> Send(PacketDataTransfer packetDataTransfer, CancellationToken ct)
    {
        var packet = new Packet(packetDataTransfer)
        {
            Sequence = GenSequence()
        };

        var key = _keyGenerator.NewKey(packet);
        if (key == null)
        {
            _logger.LogError("Can't generate response key for \"{MessageType}\"", packetDataTransfer.DataTransferPacketContent.MessageType);
            throw new OnePortApiException("Can't generate response key");
        }

        try
        {
            return await _onePortClient.SendAndWaitResponse<PacketDataTransfer>(key, packet, ct);
        }
        catch (TimeoutException e)
        {
            _logger.LogError(e, "Timeout on waiting response for \"{MessageType}\" sequence \"{Sequence}\"",
                packetDataTransfer.DataTransferPacketContent.MessageType, packet.Sequence);
            return null;
        }
    }

    private int GenSequence()
    {
        var cache = _sequence;
        _sequence += 1;

        return cache;
    }
}
