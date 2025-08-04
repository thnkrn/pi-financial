using Pi.SetMarketData.Application.Queries.FundDetail;
using Pi.SetMarketData.Application.Interfaces.FundDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundDetail;

public class GetByIdFundDetailRequestHandler : GetByIdFundDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundDetail> _fundDetailService;

    public GetByIdFundDetailRequestHandler(IMongoService<Domain.Entities.FundDetail> fundDetailService)
    {
        _fundDetailService = fundDetailService;
    }

    protected override async Task<GetByIdFundDetailResponse> Handle(GetByIdFundDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _fundDetailService.GetByIdAsync(request.id);
            return new GetByIdFundDetailResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}