using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.API.Models.Requests;

public class UpdateOrderRequest : IOrderValues
{
    [Range(0.0000001, double.MaxValue)]
    public decimal Quantity { get; init; }

    [Range(0.0000001, double.MaxValue)]
    public decimal? LimitPrice { get; init; }

    [Range(0.0000001, double.MaxValue)]
    public decimal? StopPrice { get; init; }
}
