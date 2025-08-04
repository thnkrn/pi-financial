using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Repositories;

public class SnDbContext : DbContext
{
    public SnDbContext(DbContextOptions<SnDbContext> options) : base(options)
    {
    }

    public DbSet<StockEntity> Stocks { get; set; }
    public DbSet<NoteEntity> Notes { get; set; }
    public DbSet<CashEntity> Cash { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.UseCollation("utf8mb4_0900_ai_ci");

        EntityTypeBuilder<NoteEntity> _note = mb.Entity<NoteEntity>();
        _note.HasKey(x => x.Id);
        _note.HasIndex(x => x.AccountId);
        _note.HasIndex(x => x.AccountNo);
        _note.HasIndex(x => x.Symbol);
        _note.HasIndex(x => x.Currency);

        EntityTypeBuilder<StockEntity> _stock = mb.Entity<StockEntity>();
        _stock.HasKey(x => x.Id);
        _stock.HasIndex(x => x.AccountId);
        _stock.HasIndex(x => x.AccountNo);
        _stock.HasIndex(x => x.Symbol);
        _stock.HasIndex(x => x.Currency);

        EntityTypeBuilder<CashEntity> _cash = mb.Entity<CashEntity>();
        _cash.HasKey(x => x.Id);
        _cash.HasIndex(x => x.AccountId);
        _cash.HasIndex(x => x.AccountNo);
        _cash.HasIndex(x => x.Currency);
    }
}
