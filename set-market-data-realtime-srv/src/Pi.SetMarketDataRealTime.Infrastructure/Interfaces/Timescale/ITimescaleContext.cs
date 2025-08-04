using Npgsql;

namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Timescale;

public interface ITimescaleContext : IDisposable
{
    NpgsqlConnection GetConnection();
}