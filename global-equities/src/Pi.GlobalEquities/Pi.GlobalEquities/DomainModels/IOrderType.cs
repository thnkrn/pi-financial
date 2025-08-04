namespace Pi.GlobalEquities.DomainModels;

public interface IOrderType
{
    OrderType OrderType { get; }
    OrderSide Side { get; }
    OrderDuration Duration { get; }
}
