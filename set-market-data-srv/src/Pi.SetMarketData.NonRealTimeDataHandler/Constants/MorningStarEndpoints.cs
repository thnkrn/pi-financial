namespace Pi.SetMarketData.NonRealTimeDataHandler.constants;

public static class MorningStarEndpoints
{
    // Common
    private const string companyFinancialsService = "Webservice/CompanyFinancialsService.asmx/";
    private const string financialKeyRatiosService = "Webservice/FinancialKeyRatiosService.asmx/";
    private const string globalMasterListsService = "WebService/GlobalMasterListsService.asmx/";
    private const string marketPerformanceService = "Webservice/MarketPerformanceService.asmx/";
    private const string investorRelationsService = "Webservice/InvestorRelationsService.asmx/";
    private const string globalCorporateActionsService =
        "Webservice/GlobalCorporateActionsService.asmx/";
    private const string globalStockAnalysisResearchService =
        "Webservice/GlobalStockAnalysisResearchService.asmx/";
    private const string commonQuery =
        "exchangeId={1}&identifierType={2}&identifier={3}&statementType={4}&dataType={5}&startDate={6}&endDate={7}&responseType={8}&Token={0}";
    private const string shortQuery =
        "exchangeId={1}&identifierType={2}&identifier={3}&responseType={4}&Token={0}";
    private const string periodQuery =
        "exchangeId={1}&identifierType={2}&identifier={3}&period={4}&responseType={5}&Token={0}";
    private const string excludingQuery =
        "exchangeId={1}&identifierType={2}&identifier={3}&excludingFrom={4}&excludingTo={5}&responseType={6}&Token={0}";

    // Login
    public const string login = "WSLogin/Login.asmx/Login?email={0}&password={1}";

    // CompanyFinancialsService
    public const string balanceSheet = companyFinancialsService + "GetBalanceSheet?" + commonQuery;
    public const string incomeStatement =
        companyFinancialsService + "GetIncomeStatement?" + commonQuery;
    public const string segmentSheets =
        companyFinancialsService + "GetSegmentSheets?" + commonQuery;

    // FinancialKeyRatiosService
    public const string valuationRatios =
        financialKeyRatiosService + "GetValuationRatios?" + periodQuery;
    public const string profitabilityRatios =
        financialKeyRatiosService + "GetProfitabilityRatios?" + commonQuery;

    // GlobalMasterListsService
    public const string getCompanyFinancialAvailabilityList =
        globalMasterListsService + "GetCompanyFinancialAvailabilityList?" + shortQuery;

    // MarketPerformanceService
    public const string getCurrentMarketCapitalization =
        marketPerformanceService + "GetCurrentMarketCapitalization?" + shortQuery;

    // GlobalStockAnalysisResearchService
    public const string getSharesSnapshot =
        globalStockAnalysisResearchService + "GetSharesSnapshot?" + shortQuery;

    // InvestorRelationsService
    public const string getCompanyGeneralInformation =
        investorRelationsService + "GetCompanyGeneralInformation?" + shortQuery;
    public const string getBusinessDescription =
        investorRelationsService + "GetBusinessDescription?" + shortQuery;

    // GlobalCorporateActionsService
    public const string getCashDividends =
        globalCorporateActionsService + "GetCashDividends?" + excludingQuery;
}
