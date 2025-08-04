using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

public interface IMorningStarDataHelper
{
    Task<MorningstarToken?> Login(string email, string password);

    Task<List<IncomeStatement>?> GetIncomeStatement(
        MorningStarStatementTypeRequest request,
        string? token
    );

    Task<List<ValuationRations>?> GetValuationRatios(
        MorningStarPeriodRequest request,
        string? token
    );
    Task<List<BalanceSheet>?> GetBalanceSheet(
        MorningStarStatementTypeRequest request,
        string? token
    );

    Task<List<ProfitabilityRatios>?> GetProfitabilityRatios(
        MorningStarStatementTypeRequest request,
        string? token
    );

    Task<List<CompanyFinancialAvailability>?> GetCompanyFinancialAvailabilityList(
        MorningStarExchangeIdRequest request,
        string? token
    );

    Task<List<CurrentMarketCapitalization>?> GetCurrentMarketCapitalisation(
        MorningStarExchangeIdRequest request,
        string? token
    );

    Task<SharesSnapshot?> GetSharesSnapshot(MorningStarExchangeIdRequest request, string? token);

    Task<CompanyGeneralInformation?> GetCompanyGeneralInformation(
        MorningStarExchangeIdRequest request,
        string? token
    );

    Task<List<CashDividend>?> GetCashDividends(
        MorningStarExcludingPeriodRequest request,
        string? token
    );

    Task<BusinessDescriptionEntity?> GetBusinessDescription(
        MorningStarExchangeIdRequest request,
        string? token
    );

    Task<FullStockExchangeSecurityListResponse?> GetStockExchangeSecurityList(
        MorningStarStockRequest request,
        string? token
    );
}
