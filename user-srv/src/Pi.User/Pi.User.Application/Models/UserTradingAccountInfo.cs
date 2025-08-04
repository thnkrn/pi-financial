using Pi.Common.Domain.AggregatesModel.ProductAggregate;

namespace Pi.User.Application.Models;

public record UserTradingAccountInfo(Guid? UserId, string CustomerCode, IEnumerable<TradingAccountWithProductInfo> TradingAccounts);

public record UserTradingAccountInfoWithExternalAccounts(
    string CustomerCode,
    List<TradingAccountDetailsWithExternalAccounts> TradingAccounts
);

public record TradingAccountDetailsWithExternalAccounts(
    Guid Id,
    string TradingAccountNo,
    string AccountType,
    string AccountTypeCode,
    string ExchangeMarketId,
    ProductName Product,
    List<ExternalAccountDetails> ExternalAccounts
);

public record ExternalAccountDetails(
    Guid Id,
    string Account,
    int ProviderId
);
