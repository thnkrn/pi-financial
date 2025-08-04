using System.Text;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;

namespace Pi.GlobalEquities.Services;

public class OrderReferenceIssuer : RsaSignatureIssuer, IOrderReferenceIssuer
{
    public OrderReferenceIssuer(string base64PrivateKey) : base(base64PrivateKey)
    {
    }

    public string CreateClientTag(OrderTagInfo orderTagInfo)
    {
        var unsignedClientTag = Conventions.OrderPrefix.Create(orderTagInfo.UserId, orderTagInfo.AccountId);
        var providerAccId = orderTagInfo.ProviderAccountId;
        var data = $"{unsignedClientTag}|{providerAccId}";
        var signature = Sign(data);
        var signedClientTag = $"{unsignedClientTag}.{signature}";
        return signedClientTag;
    }
}
