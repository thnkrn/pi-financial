using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalEquities.DomainModels;

public class Account : IAccount
{
    /// <summary>
    /// Pi account id, Maps to user account in user service
    /// </summary>
    [BsonId]
    public string Id { get; init; }
    public string UserId { get; init; }
    public string CustCode { get; init; }
    public string TradingAccountNo { get; init; }  //```custCode-2```
    public string VelexaAccount { get; init; }
    public bool EnableBuy { get; init; } = true;
    public bool EnableSell { get; init; } = true;
    public DateTime UpdatedAt { get; init; }

    public string GetProviderAccount(Provider provider)
    {
        if (provider != Provider.Velexa)
            throw new NotSupportedException(provider.ToString());

        return VelexaAccount;
    }

    public bool IsExpired()
    {
        var isExpired = (DateTime.UtcNow - UpdatedAt).TotalDays > 1;
        return isExpired;
    }
}
