namespace Pi.TfexService.Application.Services.UserService;

public record User(
    Guid Id,
    List<string> CustomerCodeList,
    List<string> TradingAccountNoList,
    string FirstnameTh,
    string LastnameTh,
    string FirstnameEn,
    string LastnameEn,
    string PhoneNumber,
    string Email
);

public record UserTradingAccountInfo(
    string CustomerCode,
    IEnumerable<TradingAccountDetails> TradingAccounts);


public record TradingAccountDetails(
    Guid Id,
    string TradingAccountNo,
    string AccountType,
    string AccountTypeCode,
    string ExchangeMarketId,
    string Product,
    IEnumerable<ExternalAccountDetails> ExternalAccounts
);

public record ExternalAccountDetails(
    Guid Id,
    string Account,
    int ProviderId
);