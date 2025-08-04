using Microsoft.Extensions.Configuration;
using Npgsql;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Timescale;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Timescale;

public class TimescaleContext : ITimescaleContext
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private bool _disposed;

    public TimescaleContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TimescaleDb")
                            ?? throw new ArgumentNullException(nameof(configuration),
                                "TimescaleDb connection string is not configured.");
    }

    public NpgsqlConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }

        return _connection;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }

            _disposed = true;
        }
    }

    ~TimescaleContext()
    {
        Dispose(false);
    }
}