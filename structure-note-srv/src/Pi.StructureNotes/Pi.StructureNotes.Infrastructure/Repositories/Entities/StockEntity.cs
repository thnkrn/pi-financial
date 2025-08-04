using System.ComponentModel.DataAnnotations.Schema;
using Pi.StructureNotes.Infrastructure.Helpers;

namespace Pi.StructureNotes.Infrastructure.Repositories.Entities;

public class StockEntity : EntityBase
{
    [Column(TypeName = "varchar(50)")] public string Symbol { get; init; }

    [Column(TypeName = "varchar(50)")] public string Currency { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<int>))]
    public int? Units { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<int>))]
    public int? Available { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? CostPrice { get; init; }
}
