using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pi.FundMarketData.Repositories.SqlDatabase;

public class CommonDbContext : DbContext
{
    public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options)
    {
    }

    public DbSet<Holiday> Holidays { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        EntityTypeBuilder<Holiday> _holiday = mb.Entity<Holiday>();
        _holiday.HasKey(x => x.HolidayDayGranularity);
        _holiday.HasIndex(x => x.SHolidayName);
    }
}
