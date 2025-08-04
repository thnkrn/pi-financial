namespace Pi.TfexService.API.Models.Account;

public record UpsertInitialMarginRequest(
    DateTime AsOfDate,
    List<UpsertInitialMarginRequestData> Data
);

public record UpsertInitialMarginRequestData(
    string Symbol,
    string ProductType,
    string Im);

