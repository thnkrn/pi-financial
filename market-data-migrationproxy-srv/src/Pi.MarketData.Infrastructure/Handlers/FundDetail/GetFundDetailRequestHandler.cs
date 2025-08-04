using Pi.MarketData.Application.Interfaces.FundDetail;
using Pi.MarketData.Application.Queries.FundDetail;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundDetail;

public class GetFundDetailRequestHandler : GetFundDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundDetail> _FundDetailService;

    public GetFundDetailRequestHandler(IMongoService<Domain.Entities.FundDetail> FundDetailService)
    {
        _FundDetailService = FundDetailService;
    }

    protected override async Task<GetFundDetailResponse> Handle(GetFundDetailRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _FundDetailService.GetAllAsync();
            return new GetFundDetailResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}