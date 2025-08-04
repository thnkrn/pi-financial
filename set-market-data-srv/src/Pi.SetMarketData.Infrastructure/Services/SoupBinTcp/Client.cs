using System.Net;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Pi.SetMarketData.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketData.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketData.Infrastructure.Services.SoupBinTcp.Codecs;
using Pi.SetMarketData.Infrastructure.Services.SoupBinTcp.Handlers;

namespace Pi.SetMarketData.Infrastructure.Services.SoupBinTcp;

/// <summary>
/// https://github.com/caozhiyuan/DotNetty/blob/dev/src/DotNetty.Rpc/Client/ReconnectHandler.cs
/// </summary>
public class Client : IClient
{
    private IPAddress _ipAddress = IPAddress.None;
    private LoginDetails _loginDetails = new();
    private int _port;
    private int _reconnectDelayMs;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly IClientListener _clientListener;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private IChannel? _clientChannel;
    private bool _isExplicitLogout;

    public Client(IClientListener clientListener)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _clientListener = clientListener;
        _isExplicitLogout = false;
    }

    private async Task RunClientAsync()
    {
        if (_loginDetails == null
            || string.IsNullOrEmpty(_loginDetails.UserName)
            || string.IsNullOrEmpty(_loginDetails.Password))
        {
            throw new ArgumentException("Login credentials cannot be null or empty!");
        }

        if (_ipAddress.Equals(IPAddress.None)
            || _port.Equals(0))
        {
            throw new ArgumentException("IPAddress or port needs to be configured!");
        }

        var group = new MultithreadEventLoopGroup();

        try
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, ushort.MaxValue, 0, 2, 0, 2,
                        true));
                    pipeline.AddLast(new LengthFieldPrepender(ByteOrder.BigEndian, 2, 0, false));
                    pipeline.AddLast(new MessageDecoder());
                    pipeline.AddLast(new MessageEncoder());
                    pipeline.AddLast(new IdleStateHandler(15, 1, 0));
                    pipeline.AddLast(new TimeoutHandler());
                    pipeline.AddLast(new ReconnectHandler(DoStartIfNeed));
                    pipeline.AddLast(new Handler(_loginDetails, _clientListener));
                }));

            _clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(_ipAddress, _port));
            _cancellationToken.WaitHandle.WaitOne();

            if (_clientChannel is { Active: true })
            {
                await _clientChannel.WriteAndFlushAsync(new LogoutRequest());
                await _clientChannel.CloseAsync();
                await _clientListener.OnDisconnect();
            }
        }
        catch (ConnectException ex)
        {
            Console.WriteLine(ex.Message);

            if (!_isExplicitLogout)
            {
                // close connection, if need
                if (_clientChannel != null)
                {
                    await _clientChannel.CloseAsync();
                } 
                
                // re-connection delay
                await Task.Delay(_reconnectDelayMs, _cancellationToken);
                
                // create new connection
                Start();
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            await group.ShutdownGracefullyAsync();
        }
    }

    public void Setup(IPAddress ipAddress, int port, int reconnectDelayMs, LoginDetails loginDetails)
    {
        _loginDetails = loginDetails;
        _ipAddress = ipAddress;
        _port = port;
        _reconnectDelayMs = reconnectDelayMs;
    }

    public void Start()
    {
        Task.Run(RunClientAsync, _cancellationToken);
    }

    public void Shutdown()
    {
        _isExplicitLogout = true;
        _cancellationTokenSource.Cancel();
    }

    public async Task Send(byte[] message)
    {
        if (_clientChannel is { Active: true })
        {
            await _clientChannel.WriteAndFlushAsync(new UnSequencedData(message));
        }
    }

    private bool IsChannelInactive
    {
        get
        {
            if (_clientChannel == null)
            {
                return true;
            }

            return !_clientChannel.Active;
        }
    }

    private async Task DoStartIfNeed(EndPoint socketAddress)
    {
        if (IsChannelInactive)
        {
            await _semaphoreSlim.WaitAsync(_cancellationToken);

            try
            {
                if (IsChannelInactive)
                {
                    Start();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}