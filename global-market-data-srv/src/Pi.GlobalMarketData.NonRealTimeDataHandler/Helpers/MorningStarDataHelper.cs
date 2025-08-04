using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Utils;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

public class MorningStarDataHelper(IHttpRequestHelper client) : IMorningStarDataHelper
{
    private string? _token;

    // Login
    public async Task<MorningstarToken?> Login(string email, string password)
    {
        var sURL = string.Format(
            "WSLogin/Login.asmx/Login?{0}={1}&{2}={3}&{4}={5}",
            "email",
            email,
            "password",
            password,
            "responseType",
            "Json"
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var loginResult = await AsyncResponseReader.StreamToString(response);

        if (string.IsNullOrEmpty(loginResult))
            return null;

        var tokenEntity = MorningstarToken.FromJson(loginResult);

        _token = tokenEntity?.Token;
        return tokenEntity;
    }

    // BalanceSheet
    public async Task<List<BalanceSheet>?> GetBalanceSheet(
        MorningStarStatementTypeRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.balanceSheet;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.StatementType,
            request.DataType,
            request.StartDate,
            request.EndDate,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = BalanceSheetResponse.FromJson(result);

        return mappedResult?.BalanceSheetEntityList;
    }

    // IncomeStatement
    public async Task<List<IncomeStatement>?> GetIncomeStatement(
        MorningStarStatementTypeRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.incomeStatement;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.StatementType,
            request.DataType,
            request.StartDate,
            request.EndDate,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = IncomeStatementResponse.FromJson(result);

        return mappedResult?.IncomeStatementEntityList;
    }

    // ValuationRatios
    public async Task<List<ValuationRations>?> GetValuationRatios(
        MorningStarPeriodRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.valuationRatios;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.Period,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = ValuationRationsResponse.FromJson(result);

        return mappedResult?.ValuationRatioEntityList;
    }

    // ProfitabilityRatios
    public async Task<List<ProfitabilityRatios>?> GetProfitabilityRatios(
        MorningStarStatementTypeRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.profitabilityRatios;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.StatementType,
            request.DataType,
            request.StartDate,
            request.EndDate,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = ProfitabilityRatiosResponse.FromJson(result);

        return mappedResult?.ProfitabilityEntityList;
    }

    // CompanyFinancialAvailabilityList
    public async Task<List<CompanyFinancialAvailability>?> GetCompanyFinancialAvailabilityList(
        MorningStarExchangeIdRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getCompanyFinancialAvailabilityList;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = CompanyFinancialAvailabilityResponse.FromJson(result);

        return mappedResult?.CompanyFinancialAvailabilityEntityList;
    }

    // CurrentMarketCapitalization
    public async Task<List<CurrentMarketCapitalization>?> GetCurrentMarketCapitalisation(
        MorningStarExchangeIdRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getCurrentMarketCapitalization;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = CurrentMarketCapitalizationResponse.FromJson(result);

        return mappedResult?.MarketCapitalizationEntityList;
    }

    // GetSharesSnapshot
    public async Task<SharesSnapshot?> GetSharesSnapshot(
        MorningStarExchangeIdRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getSharesSnapshot;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = SharesSnapshotResponse.FromJson(result);

        return mappedResult?.SharesSnapshotEntity;
    }

    // GetCompanyGeneralInformation
    public async Task<CompanyGeneralInformation?> GetCompanyGeneralInformation(
        MorningStarExchangeIdRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getCompanyGeneralInformation;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = CompanyGeneralInformationResponse.FromJson(result);

        return mappedResult?.CompanyInfoEntity;
    }

    // GetCashDividends
    public async Task<List<CashDividend>?> GetCashDividends(
        MorningStarExcludingPeriodRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getCashDividends;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ExcludingFrom,
            request.ExcludingTo,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = CashDividendResponse.FromJson(result);

        return mappedResult?.CashDividendEntityList;
    }

    // GetBusinessDescription
    public async Task<BusinessDescriptionEntity?> GetBusinessDescription(
        MorningStarExchangeIdRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getBusinessDescription;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            request.IdentifierType,
            request.Identifier,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = BusinessDescriptionResponse.FromJson(result);

        return mappedResult?.BusinessDescriptionEntity;
    }

    // GetStockExchangeSecurityList
    public async Task<FullStockExchangeSecurityListResponse?> GetStockExchangeSecurityList(
        MorningStarStockRequest request,
        string? token
    )
    {
        var endpoint = MorningStarEndpoints.getFullStockExchangeSecurityList;
        var sURL = string.Format(
            endpoint,
            token ?? _token,
            request.ExchangeId,
            IdentifierType.ExchangeId,
            request.Identifier,
            request.StockStatus,
            request.ResponseType
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        return FullStockExchangeSecurityListResponse.FromJson(result);
    }
}
