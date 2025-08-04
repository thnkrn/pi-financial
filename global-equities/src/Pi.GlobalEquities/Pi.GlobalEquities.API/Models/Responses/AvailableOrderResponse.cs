namespace Pi.GlobalEquities.API.Models.Responses;

public class AvailableOrderResponse
{
    public Dictionary<OrderType, IEnumerable<OrderDuration>> AvailableDuration { get; set; }
}

