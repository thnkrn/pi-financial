using System.Globalization;
using QuickFix;
using QuickFix.Transport;

namespace Pi.GlobalMarketData.Infrastructure.Services.FIX;

public class FIXClientManager : IClient
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly IFIXListener _listener;
    private string? _configFile;
    private SocketInitiator? _initiator;
    private bool _isExplicitLogout;

    public FIXClientManager(IFIXListener listener)
    {
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
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

    public void Shutdown()
    {
        _isExplicitLogout = true;
        _cancellationTokenSource.Cancel();

        if (_initiator != null && _initiator.IsLoggedOn)
        {
            _initiator.Stop();
            Console.WriteLine("FIX client stopped.");
        }
    }

    public void Send(Message message)
    {
        _listener.SendMessage(message);
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
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (!_isExplicitLogout && !cancellationToken.IsCancellationRequested)
            {
                if (_initiator != null && _initiator.IsLoggedOn) _initiator.Stop();

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
}