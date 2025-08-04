using Npgsql;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Timescale;

public interface ITimescaleContext : IDisposable
{
    NpgsqlConnection GetConnection();
}