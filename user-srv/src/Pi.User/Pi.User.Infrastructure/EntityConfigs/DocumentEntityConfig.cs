using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.Infrastructure.EntityConfigs;

public class DocumentEntityConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder
            .Property(x => x.DocumentType)
            .HasConversion<string>();
        builder.Property(x => x.FileUrl).HasMaxLength(200);
        builder.Property(x => x.FileName).HasMaxLength(100);
    }
}