using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Services.SqlServer;

public class SqlServerContext : DbContext
{
    public virtual DbSet<Security> Security { get; set; }
    public virtual DbSet<SecurityDetail> SecurityDetail { get; set; }
    public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Security>(entity =>
        {
            entity.HasKey(e => new { e.ISecurity });
        });

        modelBuilder.Entity<SecurityDetail>(entity =>
        {
            entity.HasKey(e => new { e.ISecurity });
            entity.Property(e => e.ZPar)
                .HasPrecision(16, 5);
            entity.Property(e => e.ZMultiplier)
                .HasPrecision(16, 5);
            entity.Property(e => e.ZIpo)
                .HasPrecision(17, 6);
            entity.Property(e => e.ZExercise)
                .HasPrecision(20, 12);
            entity.Property(e => e.PCoupon)
                .HasPrecision(16, 12);
        });
    }
}