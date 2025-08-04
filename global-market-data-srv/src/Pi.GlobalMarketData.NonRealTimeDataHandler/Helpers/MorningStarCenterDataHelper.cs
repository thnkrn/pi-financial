using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Utils;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;

public class MorningStarCenterDataHelper(IHttpRequestHelper client) : IMorningStarCenterDataHelper
{
    public async Task<AccessCodeResponse?> CreateAccessCode(string email, string password)
    {
        var endpoint = MorningStarCenterEndpoints.createAccessCode;
        var sURL = string.Format(endpoint, email, password, Expiration.D90.Value);

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        return AccessCodeResponse.FromJson(result);
    }

    public async Task<AccessCodeResponse?> QueryAccessCode(
        string email,
        string password,
        string accessCode
    )
    {
        var endpoint = MorningStarCenterEndpoints.queryAccessCode;
        var sURL = string.Format(endpoint, email, password, accessCode);

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        return AccessCodeResponse.FromJson(result);
    }

    public async Task<DeleteAccessCodeResponse?> DeleteAccessCode(
        string email,
        string password,
        string accessCode
    )
    {
        var endpoint = MorningStarCenterEndpoints.deleteAccessCode;
        var sURL = string.Format(endpoint, email, password, accessCode);

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        return DeleteAccessCodeResponse.FromJson(result);
    }

    public async Task<NetAssets?> GetNetAssets(MorningStarCenterApiRequest request)
    {
        var endpoint = MorningStarCenterEndpoints.NetAssets;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = NetAssetsResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<FundShareClassBasicInfo?> GetFundShareClassBasicInfo(
        MorningStarCenterApiRequest request
    )
    {
        var endpoint = MorningStarCenterEndpoints.FundShareClassBasicInfo;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = FundShareClassBasicInfoResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<CurrentPrice?> GetCurrentPrice(MorningStarCenterApiRequest request)
    {
        var endpoint = MorningStarCenterEndpoints.CurrentPrice;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = CurrentPriceResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<DailyPerformance?> GetDailyPerformance(MorningStarCenterApiRequest request)
    {
        var endpoint = MorningStarCenterEndpoints.DailyPerformance;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = DailyPerformanceResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<Yields?> GetYields(MorningStarCenterApiRequest request)
    {
        var endpoint = MorningStarCenterEndpoints.Yields;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = YieldsResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<ProspectusFees?> GetProspectusFees(MorningStarCenterApiRequest request)
    {
        var endpoint = MorningStarCenterEndpoints.ProspectusFees;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = ProspectusFeesResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }

    public async Task<InvestmentCriteria?> GetInvestmentCriteria(
        MorningStarCenterApiRequest request
    )
    {
        var endpoint = MorningStarCenterEndpoints.InvestmentCriteria;
        var sURL = string.Format(
            endpoint,
            request.IdentifierType,
            request.Identifier,
            request.ResponseTypeJson,
            request.AccessCode
        );

        var response = await client.RequestByUrl(sURL);
        if (response == null)
            return null;

        var result = await AsyncResponseReader.StreamToString(response);
        if (string.IsNullOrEmpty(result))
            return null;

        var mappedResult = InvestmentCriteriaResponse.FromJson(result);

        return mappedResult?.Data?.FirstOrDefault()?.Api;
    }
}
