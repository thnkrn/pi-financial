using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.DomainModels;

public static class OrderValidator
{
    private const int MINORUNIT = 100;
    public static IEnumerable<ValidationResult> Validate<T>(T order) where T : IOrderType, IOrderValues
    {
        var type = order.OrderType;
        var limit = order.LimitPrice;
        var stop = order.StopPrice;
        var side = order.Side;
        var duration = order.Duration;

        return type switch
        {
            OrderType.Market => ValidateMarketOrder(limit, stop),
            OrderType.Limit => ValidateLimitOrder(limit, stop),
            OrderType.StopLimit => ValidateStopLimitOrder(limit, stop),
            OrderType.Stop => ValidateStopOrder(side, limit, stop),
            OrderType.Tpsl => ValidateTpslOrder(side, limit, stop, duration),
            _ => throw new NotSupportedException(type.ToString())
        };
    }

    public static IEnumerable<ValidationResult> ValidateMarketOrder(decimal? limit, decimal? stop)
    {
        if (limit != null)
            yield return new ValidationResult("Limit price can not be set for market order");
        if (stop != null)
            yield return new ValidationResult("Stop price can not be set for market order");
    }

    public static IEnumerable<ValidationResult> ValidateLimitOrder(decimal? limit, decimal? stop)
    {
        if (limit == null)
            yield return new ValidationResult("Limit price must be set for limit order");
        if (stop != null)
            yield return new ValidationResult("Stop price can not be set for limit order");
        if (limit != null && !IsCorrectMinorUnit(limit.Value))
            yield return new ValidationResult("The limit price is in an incorrect minor unit.");
    }

    public static IEnumerable<ValidationResult> ValidateStopLimitOrder(decimal? limit, decimal? stop)
    {
        if (limit == null)
            yield return new ValidationResult("Limit price must be set for stop limit order");
        if (stop == null)
            yield return new ValidationResult("Stop price must be set for stop limit order");

        if (limit != null && !IsCorrectMinorUnit(limit.Value))
            yield return new ValidationResult("The limit price is in an incorrect minor unit.");
        if (stop != null && !IsCorrectMinorUnit(stop.Value))
            yield return new ValidationResult("The stop price is in an incorrect minor unit.");
    }

    public static IEnumerable<ValidationResult> ValidateStopOrder(OrderSide side, decimal? limit, decimal? stop)
    {
        if (limit != null)
            yield return new ValidationResult("Limit price must not be set for stop order");
        if (stop == null)
            yield return new ValidationResult("Stop price must be set for stop order");

        if (stop != null && !IsCorrectMinorUnit(stop.Value))
            yield return new ValidationResult("The stop price is in an incorrect minor unit.");
    }

    public static IEnumerable<ValidationResult> ValidateTpslOrder(OrderSide side, decimal? limit, decimal? stop, OrderDuration duration)
    {
        if (side == OrderSide.Buy)
            yield return new ValidationResult("Order side for tp/sl order must be sell only");

        if (limit == null)
            yield return new ValidationResult("Limit price must be set for tp/sl order");
        if (stop == null)
            yield return new ValidationResult("Stop price must be set for tp/sl order");

        if (limit != null && !IsCorrectMinorUnit(limit.Value))
            yield return new ValidationResult("The limit price is in an incorrect minor unit.");
        if (stop != null && !IsCorrectMinorUnit(stop.Value))
            yield return new ValidationResult("The stop price is in an incorrect minor unit.");

        if (limit <= stop)
            yield return new ValidationResult("The TakeProfit price must be higher than StopLoss price.");

        if (duration != OrderDuration.Gtc)
            yield return new ValidationResult($"{duration} is not support for stop order");
    }

    private static bool IsCorrectMinorUnit(decimal price)
    {
        if (price <= 0)
            return false;

        return (price * MINORUNIT) % 1 == 0;
    }
}
