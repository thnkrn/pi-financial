using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

/// <summary>
///     Factory class to manage the lifecycle of FixClientManager instances
/// </summary>
public interface IFixClientFactory
{
    /// <summary>
    ///     Gets or creates a FixClientManager instance
    /// </summary>
    IClient GetClient();

    /// <summary>
    ///     Releases and recreates the current client instance
    /// </summary>
    IClient RecreateClient();
}

public class FixClientFactory : IFixClientFactory
{
    private readonly string _configFile;
    private readonly object _lock = new();
    private readonly IServiceProvider _serviceProvider;
    private IClient? _client;
    private bool _disposed;

    /// <summary>
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="configFile"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FixClientFactory(IServiceProvider serviceProvider, string configFile)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configFile = configFile ?? throw new ArgumentNullException(nameof(configFile));
    }

    public IClient GetClient()
    {
        lock (_lock)
        {
            if (_client == null || _disposed) return CreateNewClient();

            // Check if client is in a valid state
            return _client.State == ClientState.Disconnected
                ? RecreateClient()
                : _client;
        }
    }

    public IClient RecreateClient()
    {
        lock (_lock)
        {
            // Dispose the old client if it exists
            if (_client != null)
                try
                {
                    // Try to shut down gracefully first
                    _client.ShutdownAsync().GetAwaiter().GetResult();
                    _client?.Dispose();
                }
                catch (Exception)
                {
                    // Ignore exceptions during disposal - we're recreating anyway
                }

            _disposed = false;
            return CreateNewClient();
        }
    }

    private IClient CreateNewClient()
    {
        var fixListener = _serviceProvider.GetRequiredService<IFixListener>();
        var client = new FixClientManager(fixListener);
        client.Setup(_configFile);
        _client = client;
        _disposed = false;
        return client;
    }
}