using Pi.Common.Cryptography;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa.OrderReferences;

public class OrderReferenceIssuer : RsaSignatureIssuer, IOrderReferenceIssuer
{
    public OrderReferenceIssuer(string base64PrivateKey) : base(base64PrivateKey)
    {
    }

    public string CreateClientTag(OrderTagInfo orderTagInfo, OrderType? orderType = null)
    {
        var unsignedClientTag = Conventions.OrderPrefix.Create(orderTagInfo.UserId, orderTagInfo.AccountId);
        var providerAccId = orderTagInfo.ProviderAccountId;
        var data = $"{unsignedClientTag}|{providerAccId}";
        var signature = Sign(data);
        var signedClientTag = $"{unsignedClientTag}.{signature}";

        if (orderType == null)
            return signedClientTag;

        var clientTag = $"{signedClientTag}.orderType={orderType}";
        return clientTag;
    }
}
