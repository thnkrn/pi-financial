namespace Pi.TfexService.Application.Models;

public record UpsertInitialMargin(
    List<InitialMarginData> ImData,
    DateTime AsOfDate
    );

public record UpsertInitialMarginResponse(bool IsSuccess);

public record InitialMarginData(
    string Symbol,
    string ProductType,
    decimal Im);

public enum ProductFamilyType
{
    FUT,
    PHY,
    OOP
}