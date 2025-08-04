using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa.OrderReferences;

public interface IOrderReferenceValidator
{
    OrderTagInfo? Extract(string clientTag, string providerAccountId);
}
