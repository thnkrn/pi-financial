using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.OnePort.TCP.Exceptions;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Generators;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;

namespace Pi.OnePort.TCP.Client;

public interface IOnePortResponseMapClient : IOnePortClient
{
    Task<Packet?> SendAndWaitResponse(string key, Packet packet, CancellationToken cancellationToken);
    Task<TPacketData> SendAndWaitResponse<TPacketData>(string key, Packet packet, CancellationToken cancellationToken)
        where TPacketData : IPacketDataOutBound;
}

public class OnePortResponseMapClient : OnePortClient, IOnePortResponseMapClient
{
    private readonly IResponseMap _responseMap;
    private readonly IResponseKeyGenerator _keyGenerator;

    public OnePortResponseMapClient(IResponseMap responseMap,
        IResponseKeyGenerator keyGenerator,
        IOptions<OnePortOptions> options,
        ILogger<OnePortResponseMapClient> logger) : base(options,
        logger)
    {
        _keyGenerator = keyGenerator;
        _responseMap = responseMap;
    }

    protected override async Task<Packet> Listen()
    {
        var packet = await base.Listen();
        var key = _keyGenerator.NewKey(packet);
        if (key != null && _responseMap.TryGetValue(key, out var value) && value == null)
        {
            _responseMap.UpdateKey(key, packet);
        }

        return packet;
    }

    public async Task Send(string key, Packet packet)
    {
        _responseMap.AddKey(key);
        await base.Send(packet);
    }

    public async Task<Packet?> SendAndWaitResponse(string key, Packet packet, CancellationToken cancellationToken)
    {
        await Send(key, packet);
        return await _responseMap.GetAndRemoveWithWaitingAsync(key, cancellationToken).WithTimeout(_options.TimeoutPeriod);
    }

    public async Task<TPacketData> SendAndWaitResponse<TPacketData>(string key, Packet packet, CancellationToken cancellationToken) where TPacketData : IPacketDataOutBound
    {
        var response = await SendAndWaitResponse(key, packet, cancellationToken);

        if (response?.Data is not TPacketData data)
        {
            throw new PacketDeserializationException($"Received unexpected response: {response?.Data?.GetType()}");
        }

        return data;
    }
}
