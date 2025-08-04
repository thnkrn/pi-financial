using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.API.Models.Requests;

public class PlaceOrderRequest : IOrderRequest
{
    [Required]
    public string AccountId { get; init; }

    [Required]
    [StringLength(20)]
    public string Venue { get; init; }

    [Required]
    [StringLength(20)]
    public string Symbol { get; init; }

    [Required]
    public required OrderType OrderType { get; init; }
    public OrderDuration Duration { get; init; } = OrderDuration.Day;

    [Required]
    public OrderSide Side { get; init; }

    [Required]
    [Range(0.0000001, double.MaxValue)]
    public decimal Quantity { get; init; }

    [Range(0.0000001, double.MaxValue)]
    public decimal? LimitPrice { get; init; }

    [Range(0.0000001, double.MaxValue)]
    public decimal? StopPrice { get; init; }

    public IOrder ToOrder(string userId = null) => new Order
    {
        Id = null,
        UserId = userId,
        AccountId = AccountId,
        Venue = Venue,
        Symbol = Symbol,
        OrderType = OrderType,
        Duration = Duration,
        Side = Side,
        Quantity = Quantity,
        LimitPrice = LimitPrice,
        StopPrice = StopPrice,
        Channel = Channel.Online,
        CreatedAt = DateTime.UtcNow
    };

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => OrderValidator.Validate(this);
}
