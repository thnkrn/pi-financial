using System.ComponentModel.DataAnnotations;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.API.Models;

public class BaseOrderRequest
{
    [Required]
    public required string TradingAccountNo { get; init; }
}

public class PlaceOrderRequest : BaseOrderRequest
{
    // Sirius use ConPrice as order type
    [Required]
    public required ConditionPrice OrderType { get; init; }

    // Sirius use Action as Side
    [Required]
    public required OrderAction Side { get; init; }

    [Required]
    public required string Symbol { get; init; }

    [Required]
    public required int Quantity { get; init; }
    public decimal? Price { get; init; }
    public Condition? Tif { get; init; } // Sirius use Tif as Condition
    public OrderFlag[]? Flag { get; init; } // Sirius use Flag with "NVDR" and "LENDING"
}

public class ChangeOrderRequest : BaseOrderRequest
{
    [Required]
    public required string OrderId { get; init; }
    [Required]
    public required decimal Price { get; init; }
    [Required]
    public required int Volume { get; init; }
    public OrderFlag[]? Flag { get; init; } // Sirius use Flag with "NVDR" and "LENDING"
}

public class CancelOrderRequest : BaseOrderRequest
{
    [Required]
    public required string OrderId { get; init; }
}

public enum OrderFlag
{
    Nvdr,
    Lending
}
