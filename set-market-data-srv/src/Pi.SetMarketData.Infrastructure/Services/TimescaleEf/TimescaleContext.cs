using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Services.TimescaleEf
{
    public static class CandleView
    {
        public const string Candle1Min = "candle_1_min";
        public const string Candle2Min = "candle_2_min";
        public const string Candle5Min = "candle_5_min";
        public const string Candle15Min = "candle_15_min";
        public const string Candle30Min = "candle_30_min";
        public const string Candle1Hour = "candle_1_hour";
        public const string Candle2Hour = "candle_2_hour";
        public const string Candle4Hour = "candle_4_hour";
        public const string Candle1Day = "candle_1_day";
        public const string Candle1Week = "candle_1_week";
        public const string Candle1Month = "candle_1_month";
    }

    public static class TechnicalIndicatorTable
    {
        public const string TechnicalIndicators1Min = "technical_indicators_1_min";
        public const string TechnicalIndicators2Min = "technical_indicators_2_min";
        public const string TechnicalIndicators5Min = "technical_indicators_5_min";
        public const string TechnicalIndicators15Min = "technical_indicators_15_min";
        public const string TechnicalIndicators30Min = "technical_indicators_30_min";
        public const string TechnicalIndicators1Hour = "technical_indicators_1_hour";
        public const string TechnicalIndicators2Hour = "technical_indicators_2_hour";
        public const string TechnicalIndicators4Hour = "technical_indicators_4_hour";
        public const string TechnicalIndicators1Day = "technical_indicators_1_day";
        public const string TechnicalIndicators1Week = "technical_indicators_1_week";
        public const string TechnicalIndicators1Month = "technical_indicators_1_month";
    }

    public class HighLowResult
    {
        public double? highest_high { get; set; }
        public double? lowest_low { get; set; }
    }
    public class CandleDateResult
    {
        public DateTime earliest_candle { get; set; }
    }

    public class TimescaleContext : DbContext
    {
        public TimescaleContext(DbContextOptions<TimescaleContext> options)
            : base(options) { }

        public DbSet<Candle1Month> Candle1Month { get; set; }
        public DbSet<Candle1Week> Candle1Week { get; set; }
        public DbSet<Candle1Day> Candle1Day { get; set; }
        public DbSet<Candle4Hour> Candle4Hour { get; set; }
        public DbSet<Candle2Hour> Candle2Hour { get; set; }
        public DbSet<Candle1Hour> Candle1Hour { get; set; }
        public DbSet<Candle30Min> Candle30Min { get; set; }
        public DbSet<Candle15Min> Candle15Min { get; set; }
        public DbSet<Candle5Min> Candle5Min { get; set; }
        public DbSet<Candle2Min> Candle2Min { get; set; }
        public DbSet<Candle1Min> Candle1Min { get; set; }
        public DbSet<TechnicalIndicators1Month> TechnicalIndicators1Month { get; set; }
        public DbSet<TechnicalIndicators1Week> TechnicalIndicators1Week { get; set; }
        public DbSet<TechnicalIndicators1Day> TechnicalIndicators1Day { get; set; }
        public DbSet<TechnicalIndicators4Hour> TechnicalIndicators4Hour { get; set; }
        public DbSet<TechnicalIndicators2Hour> TechnicalIndicators2Hour { get; set; }
        public DbSet<TechnicalIndicators1Hour> TechnicalIndicators1Hour { get; set; }
        public DbSet<TechnicalIndicators30Min> TechnicalIndicators30Min { get; set; }
        public DbSet<TechnicalIndicators15Min> TechnicalIndicators15Min { get; set; }
        public DbSet<TechnicalIndicators5Min> TechnicalIndicators5Min { get; set; }
        public DbSet<TechnicalIndicators1Min> TechnicalIndicators1Min { get; set; }
        public DbSet<TechnicalIndicators2Min> TechnicalIndicators2Min { get; set; }
        public DbSet<RealtimeMarketData> RealtimeMarketData { get; set; }

        public DbSet<HighLowResult> HighLowResult { get; set; }
        public DbSet<CandleDateResult> CandleDateResult { get; set; }
        public DbSet<RankingItem> RankingItem { get; set; }

        // Define a db function to create the hypertable
        [DbFunction("create_hypertable", Schema = "public")]
        public int TimescaleCreateHypertable()
        {
            try
            {
                var result = Database.ExecuteSqlRaw(
                    new StringBuilder()
                        .Append(
                            @"
                    SELECT create_hypertable('realtime_market_data', 'date_time', 
                        chunk_time_interval => INTERVAL '1 day', 
                        if_not_exists => TRUE, migrate_data => TRUE);
                "
                        )
                        .ToString()
                );

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring hypertable creation: {ex.Message}");
                return 0;
            }
        }

        // Override SaveChanges to ensure the hypertable is created after the regular table
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            if (result > 0)
                await Database.ExecuteSqlRawAsync(
                    new StringBuilder()
                        .Append(
                            @"
                    SELECT create_hypertable('realtime_market_data', 'date_time', 
                        chunk_time_interval => INTERVAL '1 day', 
                        if_not_exists => TRUE, migrate_data => TRUE);
                "
                        )
                        .ToString(),
                    cancellationToken
                );

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RealtimeMarketData>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.DateTime,
                    e.Symbol,
                    e.Venue
                });
                entity
                    .HasIndex(e => new
                    {
                        e.Symbol,
                        e.Venue,
                        e.DateTime
                    })
                    .HasDatabaseName("ix_symbol_venue_date_time");

                // Add a unique constraint to support efficient upserts
                entity
                    .HasIndex(e => new
                    {
                        e.DateTime,
                        e.Symbol,
                        e.Venue
                    })
                    .IsUnique()
                    .HasDatabaseName("ux_datetime_symbol_venue");

                // Configure the DateTime column as the time partitioning key
                entity.Property(e => e.DateTime).HasColumnType("timestamptz");
            });

            modelBuilder.Entity<RankingItem>()
                .HasKey(r => new { r.Date, r.Symbol, r.Venue });

            modelBuilder.HasPostgresExtension("timescaledb");
            modelBuilder
                .Entity<RealtimeMarketData>()
                .ToTable(
                    "realtime_market_data",
                    tb => tb.HasComment("TimescaleDB hypertable for realtime market data")
                );
            modelBuilder.HasDbFunction(() => TimescaleCreateHypertable());

            modelBuilder.Entity<HighLowResult>().HasNoKey();
            modelBuilder.Entity<CandleDateResult>().HasNoKey();

            base.OnModelCreating(modelBuilder);

            ConfigureCandleViews(modelBuilder);
            ConfigureIndicatorViews(modelBuilder);
        }

        private static void ConfigureCandleViews(ModelBuilder modelBuilder)
        {
            var candleConfigurations = new[]
            {
                new { EntityType = typeof(Candle1Min), ViewName = CandleView.Candle1Min },
                new { EntityType = typeof(Candle2Min), ViewName = CandleView.Candle2Min },
                new { EntityType = typeof(Candle5Min), ViewName = CandleView.Candle5Min },
                new { EntityType = typeof(Candle15Min), ViewName = CandleView.Candle15Min },
                new { EntityType = typeof(Candle30Min), ViewName = CandleView.Candle30Min },
                new { EntityType = typeof(Candle1Hour), ViewName = CandleView.Candle1Hour },
                new { EntityType = typeof(Candle2Hour), ViewName = CandleView.Candle2Hour },
                new { EntityType = typeof(Candle4Hour), ViewName = CandleView.Candle4Hour },
                new { EntityType = typeof(Candle1Day), ViewName = CandleView.Candle1Day },
                new { EntityType = typeof(Candle1Week), ViewName = CandleView.Candle1Week },
                new { EntityType = typeof(Candle1Month), ViewName = CandleView.Candle1Month }
            };

            foreach (var config in candleConfigurations)
            {
                modelBuilder.Entity(
                    config.EntityType,
                    builder =>
                    {
                        builder.ToView(config.ViewName);
                        builder.HasKey("Date", "Symbol", "Venue");
                    }
                );
            }
        }

        private static void ConfigureIndicatorViews(ModelBuilder modelBuilder)
        {
            var candleConfigurations = new[]
            {
                new
                {
                    EntityType = typeof(TechnicalIndicators1Min),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators1Min
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators2Min),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators2Min
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators5Min),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators5Min
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators15Min),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators15Min
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators30Min),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators30Min
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators1Hour),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators1Hour
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators2Hour),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators2Hour
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators4Hour),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators4Hour
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators1Day),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators1Day
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators1Week),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators1Week
                },
                new
                {
                    EntityType = typeof(TechnicalIndicators1Month),
                    TableName = TechnicalIndicatorTable.TechnicalIndicators1Month
                }
            };

            foreach (var config in candleConfigurations)
            {
                modelBuilder.Entity(
                    config.EntityType,
                    builder =>
                    {
                        builder.ToTable(config.TableName);
                        builder.HasKey("DateTime", "Symbol", "Venue");
                    }
                );
            }
        }
    }

    public class TimescaleContextFactory : IDesignTimeDbContextFactory<TimescaleContext>
    {
        public TimescaleContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TimescaleContext>();
            var connectionString = configuration
                .GetSection(ConfigurationKeys.TimescaleConnection)
                .Value;

            optionsBuilder.UseNpgsql(connectionString);

            return new TimescaleContext(optionsBuilder.Options);
        }
    }
}
