using Pi.MarketData.Application.Commands.FundDetail;
using Pi.MarketData.Application.Interfaces.FundDetail;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundDetail;

public class DeleteFundDetailRequestHandler : DeleteFundDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundDetail> _fundDetailService;

    public DeleteFundDetailRequestHandler(IMongoService<Domain.Entities.FundDetail> fundDetailService)
    {
        _fundDetailService = fundDetailService;
    }

    protected override async Task<DeleteFundDetailResponse> Handle(DeleteFundDetailRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _fundDetailService.DeleteAsync(request.id);
            return new DeleteFundDetailResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}