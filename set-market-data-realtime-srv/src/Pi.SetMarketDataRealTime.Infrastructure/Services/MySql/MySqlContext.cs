using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.MySql;

public class MySqlContext : IDisposable
{
    private readonly string _connectionString;
    private MySqlConnection? _connection;

    public MySqlContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlDb")
                            ?? throw new ArgumentNullException(nameof(configuration),
                                "MySqlDb connection string is not configured.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public MySqlConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
        }

        return _connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
    }
}