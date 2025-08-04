using System.ComponentModel.DataAnnotations.Schema;

namespace Pi.StructureNotes.Infrastructure.Repositories.Entities;

public class EntityBase
{
    [Csv.Ignore]
    [Column(TypeName = "varchar(100)")]
    public string AccountId { get; set; }

    [Column(TypeName = "varchar(100)")] public string AccountNo { get; init; }

    [Csv.Ignore] public Guid Id { get; set; } = Guid.NewGuid();

    [Csv.Ignore] public DateTime CreatedAt { get; set; }

    [Csv.Ignore] public DateTime UpdatedAt { get; set; }

    [Csv.Ignore] public DateTime? AsOf { get; set; }
}
