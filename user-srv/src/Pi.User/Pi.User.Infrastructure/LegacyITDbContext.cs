using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Infrastructure.EntityConfigs;

namespace Pi.User.Migrations;

public partial class LegacyITDbContext : DbContext
{
    public LegacyITDbContext()
    {
    }

    public LegacyITDbContext(DbContextOptions<LegacyITDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TradingAccount> TradingAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var tableName = Environment.GetEnvironmentVariable("LEGACYITDB_TRADINGACCOUNT_TABLE") ?? "tcas";
        modelBuilder.ApplyConfiguration(
            new TradingAccountV2EntityConfig(tableName));

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
