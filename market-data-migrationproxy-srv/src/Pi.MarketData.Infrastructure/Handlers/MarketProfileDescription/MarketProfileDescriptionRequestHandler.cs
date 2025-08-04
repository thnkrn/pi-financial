using Pi.MarketData.Application.Interfaces.MarketProfileDescription;
using Pi.MarketData.Application.Queries.MarketProfileDescription;
using Pi.MarketData.Application.Services.MarketData.MarketProfileDescription;
using Pi.MarketData.Domain.Models.Response.MorningStar;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketProfileDescription;

public class MarketProfileDescriptionRequestHandler
    : GetMarketProfileDescriptionRequestAbstractHandler
{
    private readonly IMongoService<BusinessDescriptionList> _businessDescriptionListService;
    private readonly IMongoService<CompanyGeneralInformation> _companyGeneralInfoService;

    public MarketProfileDescriptionRequestHandler(
        IMongoService<BusinessDescriptionList> businessDescriptionListService,
        IMongoService<CompanyGeneralInformation> companyGeneralInfoService
    )
    {
        _businessDescriptionListService = businessDescriptionListService;
        _companyGeneralInfoService = companyGeneralInfoService;
    }

    protected override async Task<PostMarketProfileDescriptionResponse> Handle(
        PostMarketProfileDescriptionRequest request,
        CancellationToken cancellationToken
    )
    {
        BusinessDescriptionList? businessDescriptionList;
        CompanyGeneralInformation? companyGeneralInfo;

        try
        {
            businessDescriptionList = await _businessDescriptionListService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );
            companyGeneralInfo = await _companyGeneralInfoService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );

            var result = MarketProfileDescriptionService.GetResult(
                businessDescriptionList,
                companyGeneralInfo
            );

            return new PostMarketProfileDescriptionResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}