using Pi.Common.Cryptography;

namespace Pi.GlobalEquities.Services;

public class OrderReferenceValidator : RsaSignatureValidator, IOrderReferenceValidator
{
    public OrderReferenceValidator(string base64PublicKey) : base(base64PublicKey)
    {
    }

    public OrderTagInfo Extract(string clientTag, string providerAccountId)
    {
        return !IsValidClientTag(clientTag, providerAccountId, out string str)
            ? null
            : Conventions.OrderPrefix.Extract(str);
    }

    private bool IsValidClientTag(string clientTag, string providerAccountId, out string str)
    {
        str = null;

        if (!Conventions.OrderPrefix.IsGeId(clientTag))
            return false;

        var data = clientTag.Split('.');
        if (data.Length < 2)
            throw new InvalidDataException($"{clientTag} is not valid.");

        var unsignedData = $"{data[0]}|{providerAccountId}";
        var isValid = Validate(unsignedData, data[1]);
        if (!isValid)
            throw new InvalidDataException($"{clientTag} is not valid.");

        //NOTE: If there is orderType in clientTag, put order type in str to extract
        //NOTE: ex: clientTag with orderType => GE01|{userId}|{accountId}.{signature}.orderType={orderType}
        //NOTE: `str` will be GE01|{userId}|{accountId}|orderType={orderType}
        str = data.Length == 3 ? $"{data[0]}|{data[2]}" : data[0];
        return true;
    }
}
