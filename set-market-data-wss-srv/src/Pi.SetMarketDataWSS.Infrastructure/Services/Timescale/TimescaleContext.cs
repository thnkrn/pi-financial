using Microsoft.Extensions.Configuration;
using Npgsql;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Timescale;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Timescale;

public class TimescaleContext : ITimescaleContext
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;

    public TimescaleContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TimescaleDb");
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
        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}