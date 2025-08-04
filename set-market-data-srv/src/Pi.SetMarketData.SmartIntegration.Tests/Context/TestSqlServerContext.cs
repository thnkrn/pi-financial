using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Infrastructure.Services.SqlServer;

namespace Pi.SetMarketData.SmartIntegration.Context;

public class TestSqlServerContext : SqlServerContext
{
    public TestSqlServerContext(DbContextOptions<SqlServerContext> options)
        : base(options)
    {
    }
}