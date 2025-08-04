using Microsoft.Extensions.Configuration;
using Npgsql;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Timescale;

namespace Pi.GlobalMarketData.Infrastructure.Services.Timescale;

public sealed class TimescaleContext : ITimescaleContext
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;

    public TimescaleContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetValue<string>(ConfigurationKeys.TimescaleConnection) ?? "";
        _connection = new NpgsqlConnection(_connectionString);
        _connection.Open();
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