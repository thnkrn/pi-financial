using Npgsql;

namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.Timescale;

public interface ITimescaleContext : IDisposable
{
    NpgsqlConnection GetConnection();
}