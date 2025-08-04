using System.ComponentModel.DataAnnotations.Schema;
using Pi.StructureNotes.Infrastructure.Helpers;

namespace Pi.StructureNotes.Infrastructure.Repositories.Entities;

public class CashEntity : EntityBase
{
    [Column(TypeName = "varchar(50)")] public string Symbol { get; init; }

    [Column(TypeName = "varchar(50)")] public string Currency { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? MarketValue { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? CostValue { get; init; }

    [Csv.TypeConverter(typeof(NumberConverter<decimal>))]
    public decimal? GainInPortfolio { get; init; }
}
