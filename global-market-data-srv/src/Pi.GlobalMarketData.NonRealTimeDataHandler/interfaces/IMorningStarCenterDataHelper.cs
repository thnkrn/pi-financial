using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

public interface IMorningStarCenterDataHelper
{
    Task<AccessCodeResponse?> CreateAccessCode(string email, string password);
    Task<AccessCodeResponse?> QueryAccessCode(string email, string password, string accessCode);

    Task<DeleteAccessCodeResponse?> DeleteAccessCode(
        string email,
        string password,
        string accessCode
    );
    Task<NetAssets?> GetNetAssets(MorningStarCenterApiRequest request);
    Task<FundShareClassBasicInfo?> GetFundShareClassBasicInfo(MorningStarCenterApiRequest request);
    Task<CurrentPrice?> GetCurrentPrice(MorningStarCenterApiRequest request);
    Task<DailyPerformance?> GetDailyPerformance(MorningStarCenterApiRequest request);
    Task<Yields?> GetYields(MorningStarCenterApiRequest request);
    Task<ProspectusFees?> GetProspectusFees(MorningStarCenterApiRequest request);
    Task<InvestmentCriteria?> GetInvestmentCriteria(MorningStarCenterApiRequest request);
}
