using System.ComponentModel.DataAnnotations;
using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Application.Configs;

public class InstrumentOrderOptions
{
    public const string Options = "InstrumentSequence";
    [Required]
    public required IEnumerable<InstrumentCategory> Sequence { get; init; }
}