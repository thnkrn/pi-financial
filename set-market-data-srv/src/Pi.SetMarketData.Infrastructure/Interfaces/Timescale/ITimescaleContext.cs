using Npgsql;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Timescale;

public interface ITimescaleContext : IDisposable
{
    NpgsqlConnection GetConnection();
}