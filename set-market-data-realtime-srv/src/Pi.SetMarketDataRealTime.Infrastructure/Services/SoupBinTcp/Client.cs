using System.Net;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Codecs;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Handlers;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp;

public sealed class Client : IClient
{
    private readonly IClientListener _clientListener;
    private readonly ILogger<Client> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IChannel? _clientChannel;
    private CancellationTokenSource? _cts;

    private bool _disposed;
    private MultithreadEventLoopGroup? _group;
    private IPAddress _ipAddress = IPAddress.None;
    private LoginDetails _loginDetails = new();
    private int _port;
    private int _reconnectDelayMs;
    private volatile ClientState _state = ClientState.Disconnected;

    /// <summary>
    /// </summary>
    /// <param name="clientListener"></param>
    /// <param name="loggerFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Client(IClientListener clientListener, ILoggerFactory loggerFactory)
    {
        _clientListener = clientListener ?? throw new ArgumentNullException(nameof(clientListener));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<Client>();
    }

    public ClientState State => _state;

    public async Task SetupAsync(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails)
    {
        ThrowIfDisposed();

        _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        _port = port;
        _reconnectDelayMs = reconnectDelayMs;
        _loginDetails = loginDetails ?? throw new ArgumentNullException(nameof(loginDetails));

        await Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Disconnected)
            throw new InvalidOperationException("Client is already started or starting.");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _group = new MultithreadEventLoopGroup();
        _state = ClientState.Connecting;

        await ConnectAsync(_cts.Token).ConfigureAwait(false);
    }

    public async Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        try
        {
            if (_state is ClientState.ShuttingDown or ClientState.Disconnected) return;

            _state = ClientState.ShuttingDown;
            if (_cts != null) await _cts.CancelAsync();

            await LogoutAsync(cancellationToken).ConfigureAwait(false);

            if (_group != null)
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                    .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }

        _state = ClientState.Disconnected;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Connecting)
            throw new InvalidOperationException("Client is not in a connecting state.");

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            while (!cancellationToken.IsCancellationRequested && _state == ClientState.Connecting)
                try
                {
                    var bootstrap = new Bootstrap()
                        .Group(_group)
                        .Channel<TcpSocketChannel>()
                        .Option(ChannelOption.TcpNodelay, true)
                        .Handler(new ActionChannelInitializer<ISocketChannel>(InitializeChannel));

                    _clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(_ipAddress, _port))
                        .ConfigureAwait(false);
                    _state = ClientState.Connected;
                    _logger.LogDebug("Connected successfully to {IpAddress}:{Port}", _ipAddress, _port);
                    return;
                }
                catch (ConnectException ex)
                {
                    _logger.LogWarning(ex, "Failed to connect to {IpAddress}:{Port}. Retrying in {Delay}ms...",
                        _ipAddress, _port, _reconnectDelayMs);
                    await Task.Delay(_reconnectDelayMs, cancellationToken).ConfigureAwait(false);
                }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SendAsync(byte[]? message, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Connected) throw new InvalidOperationException("Client is not connected.");

        if (_clientChannel?.Active == true)
            await _clientChannel.WriteAndFlushAsync(new UnSequencedData(message)).ConfigureAwait(false);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Connected) return;

        if (_clientChannel?.Active == true)
        {
            await _clientChannel.WriteAndFlushAsync(new LogoutRequest()).ConfigureAwait(false);
            await Task.Delay(10, cancellationToken).ConfigureAwait(false);
            await _clientChannel.CloseAsync().ConfigureAwait(false);
        }

        await _clientListener.OnDisconnect(false).ConfigureAwait(false);
        _state = ClientState.Disconnected;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void InitializeChannel(IChannel channel)
    {
        var pipeline = channel.Pipeline;
        pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, ushort.MaxValue, 0, 2, 0, 2, true));
        pipeline.AddLast(new LengthFieldPrepender(ByteOrder.BigEndian, 2, 0, false));
        pipeline.AddLast(new MessageDecoder());
        pipeline.AddLast(new MessageEncoder());
        pipeline.AddLast(new IdleStateHandler(15, 1, 0));
        pipeline.AddLast(new TimeoutHandler(_loggerFactory.CreateLogger<TimeoutHandler>()));
        pipeline.AddLast(new Handler(_loginDetails, _clientListener, _loggerFactory.CreateLogger<Handler>(),
            HandleUnexpectedDisconnection));
    }

    private void HandleUnexpectedDisconnection(bool isUnexpectedDisconnection)
    {
        switch (_state)
        {
            case ClientState.ShuttingDown:
            case ClientState.Disconnected:
                return;
            default:
                _state = ClientState.Disconnected;
                Task.Run(
                    async () => await _clientListener.OnDisconnect(isUnexpectedDisconnection).ConfigureAwait(false));
                break;
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            _cts?.Dispose();
            _semaphore.Dispose();
            _group?.ShutdownGracefullyAsync().GetAwaiter().GetResult();
        }

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(GetType().FullName);
    }
}