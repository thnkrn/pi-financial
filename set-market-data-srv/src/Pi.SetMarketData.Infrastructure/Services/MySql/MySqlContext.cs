using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Pi.SetMarketData.Infrastructure.Services.MySql;

public class MySqlContext : IDisposable
{
    private readonly string _connectionString;
    private MySqlConnection _connection;

    public MySqlContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlDb");
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

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}