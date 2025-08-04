using System.Text.Json.Serialization;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.API.Models;

public record CreateUserAccountRequest
{
    [JsonRequired]
    public string UserAccountId { get; init; } = "";

    [JsonRequired]
    public UserAccountType UserAccountType { get; init; } = UserAccountType.CashWallet;
}