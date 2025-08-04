using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public interface IOrderValues
{
    decimal Quantity { get; }
    decimal? LimitPrice { get; }
    decimal? StopPrice { get; }
}


