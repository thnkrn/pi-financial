using System.Globalization;
using Microsoft.Extensions.Logging;
using QuickFix;
using QuickFix.Transport;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Fix;

public class FixClientManager : IClient, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IFixListener _listener;
    private readonly ILogger<FixClientManager> _logger;
    private string? _configFile;
    private bool _disposed;
    private SocketInitiator? _initiator;
    private bool _isExplicitLogout;

    /// <summary>
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FixClientManager(IFixListener listener, ILogger<FixClientManager> logger)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _isExplicitLogout = false;
    }

    public void Setup(string configFile)
    {
        _configFile = configFile ?? throw new ArgumentNullException(nameof(configFile));
    }

    public void Start()
    {
        Task.Run(() => RunClientAsync(_cancellationTokenSource.Token));
    }

    public void Send(Message message)
    {
        _listener.SendMessage(message);
    }

    public void Shutdown()
    {
        _isExplicitLogout = true;
        _cancellationTokenSource.Cancel();

        if (_initiator is { IsLoggedOn: true })
        {
            _initiator.Stop();
            _logger.LogDebug("FIX client stopped.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task RunClientAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_configFile))
            throw new ArgumentException("Configuration file cannot be null or empty!");

        var settings = new SessionSettings(_configFile);
        var storeFactory = new FileStoreFactory(settings);
        var logFactory = new ScreenLogFactory(settings);

        try
        {
            _initiator = new SocketInitiator(_listener, storeFactory, settings, logFactory);
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            _initiator.Start();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while running the FIX client: {Message}", ex.Message);
            if (!_isExplicitLogout && !cancellationToken.IsCancellationRequested)
            {
                if (_initiator is { IsLoggedOn: true }) _initiator.Stop();

                // Reconnect after a delay
                await Task.Delay(5000, cancellationToken); // Wait 5 seconds before reconnecting
                if (!cancellationToken.IsCancellationRequested)
                    Start();
            }
            else
            {
                throw;
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources
            Shutdown();
            _cancellationTokenSource.Dispose();
        }

        // Dispose unmanaged resources (if any)
        _disposed = true;
    }
}