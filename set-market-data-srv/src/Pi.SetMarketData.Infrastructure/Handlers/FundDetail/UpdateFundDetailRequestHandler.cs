using Pi.SetMarketData.Application.Commands.FundDetail;
using Pi.SetMarketData.Application.Interfaces.FundDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundDetail;

public class UpdateFundDetailRequestHandler : UpdateFundDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundDetail> _FundDetailService;

    public UpdateFundDetailRequestHandler(IMongoService<Domain.Entities.FundDetail> FundDetailService)
    {
        _FundDetailService = FundDetailService;
    }

    protected override async Task<UpdateFundDetailResponse> Handle(UpdateFundDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _FundDetailService.UpdateAsync(request.id, request.FundDetail);
            return new UpdateFundDetailResponse(true, request.FundDetail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}