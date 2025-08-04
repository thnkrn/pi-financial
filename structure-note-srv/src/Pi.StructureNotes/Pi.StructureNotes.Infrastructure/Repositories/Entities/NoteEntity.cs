using System.ComponentModel.DataAnnotations.Schema;
using Pi.StructureNotes.Infrastructure.Helpers;

namespace Pi.StructureNotes.Infrastructure.Repositories.Entities;

public class NoteEntity : EntityBase
{
    [Column(TypeName = "varchar(100)")] public string ISIN { get; init; }

    [Column(TypeName = "varchar(100)")] public string Symbol { get; init; }

    [Column(TypeName = "varchar(100)")] public string Currency { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? MarketValue { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? CostValue { get; init; }

    [Column(TypeName = "varchar(200)")] public string Type { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? Yield { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? Rebate { get; init; }

    [Column(TypeName = "varchar(400)")] public string Underlying { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<int>))]
    public int? Tenors { get; init; }

    public DateTime TradeDate { get; init; }
    public DateTime IssueDate { get; init; }
    public DateTime ValuationDate { get; init; }

    [Column(TypeName = "varchar(400)")] public string Issuer { get; init; }
}
