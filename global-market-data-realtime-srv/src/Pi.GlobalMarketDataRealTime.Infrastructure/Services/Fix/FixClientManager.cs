using DotNetty.Transport.Channels;
using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

public sealed class FixClientManager : IClient
{
    private readonly bool _isExplicitLogout;
    private readonly IFixListener _listener;
    private string? _configFile;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    private MultithreadEventLoopGroup? _group;
    private SocketInitiator? _initiator;
    private volatile ClientState _state = ClientState.Disconnected;

    /// <summary>
    /// </summary>
    /// <param name="listener"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FixClientManager(IFixListener listener)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        _isExplicitLogout = false;
    }

    public ClientState State => _state;

    public void Setup(string configFile)
    {
        _configFile = configFile ?? throw new ArgumentNullException(nameof(configFile));
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        _listener.IsInitialService = cancellationToken == CancellationToken.None;

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

        if (_state is ClientState.ShuttingDown or ClientState.Disconnected) return;

        _state = ClientState.ShuttingDown;
        if (_cts != null) await _cts.CancelAsync();
        _initiator?.Stop();

        await LogoutAsync(cancellationToken).ConfigureAwait(false);

        if (_group != null)
            await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                .ConfigureAwait(false);

        _state = ClientState.Disconnected;
    }

    public async Task Reset(CancellationToken cancellationToken = default)
    {
        // Clean up existing resources first
        try
        {
            if (_initiator != null)
            {
                _initiator.Stop();
                _initiator.Dispose();
                _initiator = null;
            }

            if (_cts != null)
            {
                await _cts.CancelAsync();
                _cts.Dispose();
                _cts = null;
            }

            if (_group != null)
            {
                await _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                _group = null;
            }
        }
        catch (Exception ex)
        {
            // Log but continue with reset
            Console.WriteLine($"Error during cleanup phase of Reset: {ex.Message}");
        }

        // Reset the state
        _disposed = false;
        _state = ClientState.Disconnected;

        // Create new instances
        _cts = new CancellationTokenSource();
        _group = new MultithreadEventLoopGroup();

        // Signal completion
        await Task.CompletedTask;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ValidateStateAndConfig();

        var settings = new SessionSettings(_configFile);
        var storeFactory = new FileStoreFactory(settings);
        var logFactory = new ScreenLogFactory(settings);

        while (!cancellationToken.IsCancellationRequested && _state == ClientState.Connecting)
            try
            {
                await InitiateConnectionAsync(settings, storeFactory, logFactory);
                _state = ClientState.Connected;
                return;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (Exception)
            {
                if (await ShouldRetryConnectionAsync(cancellationToken))
                    await Task.Delay(5000, cancellationToken); // Wait 5 seconds before reconnecting
                else
                    throw;
            }
    }

    public async Task SendAsync(Message message, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Connected)
            throw new InvalidOperationException("Client is not connected.");

        _listener.SendMessage(message);
        await Task.CompletedTask;
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_state != ClientState.Connected) return;

        _state = ClientState.Disconnected;
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<bool> CheckListenerSession()
    {
        await Task.CompletedTask;
        return _listener.CheckSession();
    }

    private void ValidateStateAndConfig()
    {
        if (_state != ClientState.Connecting)
            throw new InvalidOperationException("Client is not in a connecting state.");

        if (string.IsNullOrEmpty(_configFile))
            throw new ArgumentException("Configuration file cannot be null or empty!");
    }

    private async Task InitiateConnectionAsync(SessionSettings settings, IMessageStoreFactory storeFactory,
        ILogFactory logFactory)
    {
        ConfigureSocketSettings(settings);

        _initiator = new SocketInitiator(_listener, storeFactory, settings, logFactory);
        _initiator.Start();
        await Task.CompletedTask; // To maintain async signature
    }

    private static void ConfigureSocketSettings(SessionSettings settings)
    {
        foreach (var sessionId in settings.GetSessions())
            try
            {
                // Create a settings dictionary for all the socket settings
                var socketSettings = new SettingsDictionary();

                // Add all settings to the dictionary
                socketSettings.SetString("SocketNodelay", "Y");
                socketSettings.SetString("SocketKeepAlive", "Y");
                socketSettings.SetString("SocketTimeout", "60");
                socketSettings.SetString("PersistMessages", "Y");
                socketSettings.SetString("HeartBtInt", "30");
                socketSettings.SetString("ReconnectInterval", "10");

                // Apply all settings at once to the session
                settings.Set(sessionId, socketSettings);
            }
            catch (Exception ex)
            {
                // Log but continue - don't prevent connection if settings can't be applied
                Console.WriteLine($"Error configuring socket settings for session {sessionId}: {ex.Message}");
            }
    }

    private async Task<bool> ShouldRetryConnectionAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (!_isExplicitLogout && !cancellationToken.IsCancellationRequested)
        {
            if (_initiator is { IsLoggedOn: true }) _initiator.Stop();
            return true;
        }

        return false;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources
            _cts?.Dispose();
            _group?.ShutdownGracefullyAsync().GetAwaiter().GetResult();
            _initiator?.Dispose();
        }

        _cts = null;
        _group = null;
        _initiator = null;

        _disposed = true;
    }

    ~FixClientManager()
    {
        Dispose(false);
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(GetType().FullName);
    }
}