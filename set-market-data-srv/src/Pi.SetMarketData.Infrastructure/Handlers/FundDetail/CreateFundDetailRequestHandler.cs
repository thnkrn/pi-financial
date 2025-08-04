using Pi.SetMarketData.Application.Commands.FundDetail;
using Pi.SetMarketData.Application.Interfaces.FundDetail;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundDetail;

public class CreateFundDetailRequestHandler : CreateFundDetailRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundDetail> _fundDetailService;

    public CreateFundDetailRequestHandler(IMongoService<Domain.Entities.FundDetail> fundDetailService)
    {
        _fundDetailService = fundDetailService;
    }

    protected override async Task<CreateFundDetailResponse> Handle(CreateFundDetailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _fundDetailService.CreateAsync(request.FundDetail);
            return new CreateFundDetailResponse(true, request.FundDetail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}