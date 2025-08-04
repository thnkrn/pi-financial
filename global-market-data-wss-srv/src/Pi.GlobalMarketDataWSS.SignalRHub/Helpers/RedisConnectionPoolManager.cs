using System.Collections.Concurrent;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Helpers;

/// <summary>
///     Manages a pool of Redis connections for improved performance and reliability
/// </summary>
public sealed class RedisConnectionPoolManager : IDisposable
{
    private readonly ConcurrentBag<PooledConnection> _connectionPool;
    private readonly SemaphoreSlim _connectionSemaphore;
    private readonly TimeSpan _connectionTtl;
    private readonly ILogger _logger;
    private readonly Timer _maintenanceTimer;
    private readonly int _maxPoolSize;
    private readonly int _minPoolSize;
    private readonly ConfigurationOptions _redisConfig;
    private int _currentPoolSize;
    private bool _isDisposed;

    public RedisConnectionPoolManager(
        ConfigurationOptions redisConfig,
        ILogger logger,
        int minPoolSize = 5,
        int maxPoolSize = 20,
        TimeSpan? connectionTtl = null)
    {
        _redisConfig = redisConfig ?? throw new ArgumentNullException(nameof(redisConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _minPoolSize = minPoolSize;
        _maxPoolSize = maxPoolSize;
        _connectionTtl = connectionTtl ?? TimeSpan.FromMinutes(30);
        _connectionSemaphore = new SemaphoreSlim(_maxPoolSize, _maxPoolSize);
        _connectionPool = [];
        _currentPoolSize = 0;

        InitializeMinimumPool();

        // Set up a maintenance timer that runs every 5 minutes
        _maintenanceTimer = new Timer(PerformPoolMaintenance, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public int CurrentPoolSize => _currentPoolSize;

    public void Dispose()
    {
        if (_isDisposed) return;

        _isDisposed = true;

        // Dispose the maintenance timer
        _maintenanceTimer.Dispose();

        while (_connectionPool.TryTake(out var pooledConnection))
            try
            {
                if (pooledConnection.Connection.IsValueCreated)
                {
                    pooledConnection.Connection.Value.Close();
                    pooledConnection.Connection.Value.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing Redis connection");
            }

        _connectionSemaphore.Dispose();
        _logger.LogInformation("Redis connection pool disposed");
    }

    private void InitializeMinimumPool()
    {
        _logger.LogInformation("Initializing Redis connection pool with minimum size {Size}", _minPoolSize);

        for (var i = 0; i < _minPoolSize; i++)
        {
            var pooledConnection = new PooledConnection(CreateNewConnection());
            _connectionPool.Add(pooledConnection);
            Interlocked.Increment(ref _currentPoolSize);
        }

        _logger.LogInformation("Redis connection pool initialized with {Size} connections", _currentPoolSize);
    }

    private Lazy<ConnectionMultiplexer> CreateNewConnection()
    {
        return new Lazy<ConnectionMultiplexer>(() =>
        {
            _logger.LogDebug("Creating new Redis connection");
            try
            {
                var connection = ConnectionMultiplexer.Connect(_redisConfig);
                connection.ConnectionFailed += (_, args) =>
                {
                    _logger.LogError("Redis connection failed: {EndPoint}, {FailureType}",
                        args.EndPoint, args.FailureType);
                };

                connection.ConnectionRestored += (_, args) =>
                {
                    _logger.LogInformation("Redis connection restored: {EndPoint}", args.EndPoint);
                };

                connection.ErrorMessage += (_, args) =>
                {
                    _logger.LogWarning("Redis error message: {Message}", args.Message);
                };

                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Redis connection");
                throw new InvalidOperationException(ex.Message);
            }
        }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<ConnectionMultiplexer> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connectionPool.TryTake(out var pooledConnection))
        {
            if (!pooledConnection.Connection.IsValueCreated ||
                pooledConnection.Connection is { IsValueCreated: true, Value.IsConnected: false } ||
                pooledConnection.IsExpired(_connectionTtl))
            {
                _logger.LogWarning("Recycling expired or disconnected Redis connection");
                TryDispose(pooledConnection.Connection);
                Interlocked.Decrement(ref _currentPoolSize);
            }
            else
            {
                return pooledConnection.Connection.Value;
            }
        }

        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check if a connection became available while waiting
            if (_connectionPool.TryTake(out pooledConnection))
            {
                if (!pooledConnection.Connection.IsValueCreated ||
                    pooledConnection.Connection is { IsValueCreated: true, Value.IsConnected: false } ||
                    pooledConnection.IsExpired(_connectionTtl))
                {
                    _logger.LogWarning("Recycling expired or disconnected Redis connection (after wait)");
                    TryDispose(pooledConnection.Connection);
                    Interlocked.Decrement(ref _currentPoolSize);
                }
                else
                {
                    return pooledConnection.Connection.Value;
                }
            }

            if (_currentPoolSize < _maxPoolSize)
            {
                var newConnection = new PooledConnection(CreateNewConnection());
                Interlocked.Increment(ref _currentPoolSize);
                return newConnection.Connection.Value;
            }

            _logger.LogWarning("Redis connection pool reached maximum size of {MaxSize}", _maxPoolSize);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var waitForConnectionTask = Task.Run(async () =>
            {
                while (!_connectionPool.TryTake(out pooledConnection) && !cancellationToken.IsCancellationRequested)
                    await Task.Delay(100, cancellationToken);
                return pooledConnection;
            }, cancellationToken);

            var completedTask = await Task.WhenAny(waitForConnectionTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                _logger.LogWarning(
                    "Timed out waiting for an available Redis connection from the pool. Creating a new connection instead.");

                // Instead of throwing an exception, create a new connection as a fallback
                var fallbackConnection = CreateNewConnection();

                // Only increment the counter if we're still under the absolute maximum
                // This might temporarily exceed maxPoolSize but prevents connection starvation
                if (_currentPoolSize < _maxPoolSize * 1.2) // Allow 20% overflow in emergency
                    Interlocked.Increment(ref _currentPoolSize);

                return fallbackConnection.Value;
            }

            var readyConnection = await waitForConnectionTask;
            if (readyConnection == null ||
                !readyConnection.Connection.IsValueCreated ||
                readyConnection.Connection is { IsValueCreated: true, Value.IsConnected: false } ||
                readyConnection.IsExpired(_connectionTtl))
            {
                _logger.LogWarning("Recycling expired or disconnected Redis connection (after wait)");
                TryDispose(readyConnection?.Connection);
                Interlocked.Decrement(ref _currentPoolSize);

                // Create a new connection when all existing ones are invalid
                var newLazyConnection = CreateNewConnection();
                Interlocked.Increment(ref _currentPoolSize);
                return newLazyConnection.Value;
            }

            return readyConnection.Connection.Value;
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    public void ReturnConnection(ConnectionMultiplexer? connection)
    {
        if (connection == null) return;

        // Check if the connection is valid before returning it to the pool
        if (!connection.IsConnected)
        {
            _logger.LogWarning("Not returning disconnected Redis connection to pool");
            TryDispose(new Lazy<ConnectionMultiplexer>(() => connection));
            return;
        }

        // Create a wrapped connection that preserves the existing multiplexer
        // Using a wrapper that avoids creating a new connection
        var wrappedConnection =
            new Lazy<ConnectionMultiplexer>(() => connection, LazyThreadSafetyMode.ExecutionAndPublication);
        _connectionPool.Add(new PooledConnection(wrappedConnection, DateTime.UtcNow));
    }

    public async Task<IDatabase> GetDatabaseAsync(int db = -1, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return connection.GetDatabase(db);
    }

    public async Task<ISubscriber> GetSubscriberAsync(CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return connection.GetSubscriber();
    }

    private void TryDispose(Lazy<ConnectionMultiplexer>? lazyConnection)
    {
        if (lazyConnection == null) return;
        if (lazyConnection.IsValueCreated)
            try
            {
                lazyConnection.Value.Close();
                lazyConnection.Value.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing Redis connection");
            }
    }

    /// <summary>
    ///     Performs periodic maintenance on the connection pool to remove expired connections
    /// </summary>
    private void PerformPoolMaintenance(object? state)
    {
        try
        {
            _logger.LogDebug("Performing Redis connection pool maintenance");

            var tempList = new List<PooledConnection>();
            var removedCount = 0;

            // Take all connections from the pool
            while (_connectionPool.TryTake(out var connection)) tempList.Add(connection);

            // Check each connection and only return valid ones to the pool
            foreach (var conn in tempList)
                if (conn.Connection.IsValueCreated &&
                    (conn.IsExpired(_connectionTtl) || !conn.Connection.Value.IsConnected))
                {
                    TryDispose(conn.Connection);
                    Interlocked.Decrement(ref _currentPoolSize);
                    removedCount++;
                }
                else
                {
                    _connectionPool.Add(conn);
                }

            // Ensure we maintain the minimum pool size
            var toAdd = Math.Max(0, _minPoolSize - _currentPoolSize);
            for (var i = 0; i < toAdd; i++)
            {
                var pooledConnection = new PooledConnection(CreateNewConnection());
                _connectionPool.Add(pooledConnection);
                Interlocked.Increment(ref _currentPoolSize);
            }

            _logger.LogInformation(
                "Redis pool maintenance complete. Removed {RemovedCount} expired connections, added {AddedCount} new connections. Current pool size: {CurrentSize}",
                removedCount, toAdd, _currentPoolSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Redis connection pool maintenance");
        }
    }

    private sealed class PooledConnection
    {
        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        public PooledConnection(Lazy<ConnectionMultiplexer> connection)
        {
            Connection = connection;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        ///     Create a pooled connection with an existing connection and specific creation time
        /// </summary>
        /// <param name="connection">The existing connection</param>
        /// <param name="createdAt">The creation timestamp</param>
        public PooledConnection(Lazy<ConnectionMultiplexer> connection, DateTime createdAt)
        {
            Connection = connection;
            CreatedAt = createdAt;
        }

        public Lazy<ConnectionMultiplexer> Connection { get; }
        private DateTime CreatedAt { get; }

        public bool IsExpired(TimeSpan ttl)
        {
            return DateTime.UtcNow - CreatedAt > ttl;
        }
    }
}