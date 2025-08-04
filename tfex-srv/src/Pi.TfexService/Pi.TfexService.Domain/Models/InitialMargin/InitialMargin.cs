using Pi.TfexService.Domain.SeedWork;

namespace Pi.TfexService.Domain.Models.InitialMargin;

public class InitialMargin : BaseEntity
{
    public Guid Id { get; set; }
    public required string Symbol { get; set; }
    public required string ProductType { get; set; }
    public decimal Im { get; set; }
    public decimal ImOutright { get; set; }
    public decimal ImSpread { get; set; }
    public DateOnly AsOfDate { get; set; }
}