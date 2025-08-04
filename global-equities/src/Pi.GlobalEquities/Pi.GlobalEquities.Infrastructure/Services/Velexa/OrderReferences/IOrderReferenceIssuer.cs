using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa.OrderReferences;

public interface IOrderReferenceIssuer
{
    string CreateClientTag(OrderTagInfo orderTagInfo, OrderType? orderType = null);
}
