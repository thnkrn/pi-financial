using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

public interface IMorningStarDataService
{
    Task MorningStarService(MorningStarDataHelper helper);

    Task CallIncomeStatement(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallBalanceSheet(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallProfitabilityRatios(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallValuationRatios(
        MorningStarDataHelper helper,
        MorningStarPeriodRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallCompanyFinancialAvailabilityList(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallCurrentMarketCapitalization(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallSharesSnapshot(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    );

    Task CallCompanyGeneralInformation(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    );
    Task CallBusinessDescription(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    );
    Task CallCashDividends(
        MorningStarDataHelper helper,
        MorningStarExcludingPeriodRequest request,
        MorningStarStocks morningStarStocks
    );
}
