using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.OnePort.TCP.Exceptions;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Models;
using Pi.OnePort.TCP.Models.Packets;

namespace Pi.OnePort.TCP.Client;

public interface IOnePortClient : IDisposable
{
    Task Connect(CancellationToken cancellationToken = default);
    Task Disconnect();
    Task Reconnect();
    Task Send(Packet packet);
    Task<Packet?> ListenWithWaiting(CancellationToken cancellationToken);
    Task<Packet?> SendAndWaitResponse(Packet packet, CancellationToken cancellationToken);
    Task<TPacketData> SendAndWaitResponse<TPacketData>(Packet packet, CancellationToken cancellationToken)
        where TPacketData : IPacketDataOutBound;
}

public class OnePortClient : IOnePortClient
{
    protected readonly OnePortOptions _options;
    private TcpClient _tcpClient;
    private readonly ILogger<OnePortClient> _logger;

    public OnePortClient(IOptions<OnePortOptions> options, ILogger<OnePortClient> logger)
    {
        _options = options.Value;
        _tcpClient = new TcpClient()
        {
            SendTimeout = _options.SendTimeout
        };
        _logger = logger;
    }

    public void Dispose() => _tcpClient.Dispose();

    public async Task Connect(CancellationToken cancellationToken = default)
    {
        if (_tcpClient.Connected)
        {
            return;
        }

        _logger.LogDebug("Creating Connection");
        var remoteEp = new IPEndPoint(IPAddress.Parse(_options.Ip), _options.Port);
        await _tcpClient.ConnectAsync(remoteEp, cancellationToken).AsTask().WithTimeout(_options.TimeoutPeriod);
        if (!_tcpClient.Connected)
        {
            throw new SocketException((int)SocketError.TimedOut); // Connection timed out.
        }
        _logger.LogDebug("Connection Created Successfully");
    }

    public async Task Reconnect()
    {
        _tcpClient.Dispose();
        _tcpClient = new TcpClient()
        {
            SendTimeout = _options.SendTimeout
        };
        await Connect();
    }

    public async Task Disconnect()
    {
        if (_tcpClient.Connected)
        {
            _logger.LogDebug("Still connected");
            await Send(new Packet(new PacketLogout()));
            _logger.LogDebug("Logged out");
        }
        Dispose();
    }

    public virtual async Task Send(Packet packet)
    {
        await Connect();
        var content = packet.Serialize();

        var payloadSize = BitConverter.GetBytes(Convert.ToInt16(content.Length));
        if (BitConverter.IsLittleEndian) Array.Reverse(payloadSize);

        var payload = payloadSize.Concat(content).ToArray();

        var payloadStr = Packet.Encoding.GetString(payload);
        _logger.LogDebug("Send {PacketType} packet with payload: \"{PayloadStr}\"", packet.Data.GetType().ToString(), payloadStr);

        await _tcpClient.GetStream().WriteAsync(payload);
    }

    public async Task<Packet?> ListenWithWaiting(CancellationToken cancellationToken)
    {
        await Connect(cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_tcpClient.GetStream().DataAvailable)
            {
                _logger.LogTrace("Waiting for data");
                await Task.Delay(100, cancellationToken);
                continue;
            }

            return await Listen();
        }

        return null;
    }

    protected virtual async Task<Packet> Listen()
    {
        var response = await _tcpClient.Read();
        var packetStr = Packet.Encoding.GetString(response);

        try
        {
            var packet = Packet.Deserialize(packetStr);
            _logger.LogDebug("Received {PacketType} packet with payload: \"{PacketStr}\"", packet.Data.GetType().ToString(), packetStr);

            return packet;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while trying to deserialize packet, [{PacketStr}]", packetStr);
            throw new PacketDeserializationException($"Error occurred while trying to deserialize packet, [{packetStr}]", e);
        }
    }

    public virtual async Task<Packet?> SendAndWaitResponse(Packet packet, CancellationToken cancellationToken)
    {
        await Send(packet);

        return await ListenWithWaiting(cancellationToken).WithTimeout(_options.TimeoutPeriod);
    }

    public async Task<TPacketData> SendAndWaitResponse<TPacketData>(Packet packet, CancellationToken cancellationToken) where TPacketData : IPacketDataOutBound
    {
        var response = await SendAndWaitResponse(packet, cancellationToken);

        if (response?.Data is not TPacketData data)
        {
            throw new PacketDeserializationException($"Received unexpected response: {response?.Data?.GetType()}");
        }

        return data;
    }
}
