using Pi.SetMarketData.Application.Commands.Financial;
using Pi.SetMarketData.Application.Interfaces.Financial;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Financial;

public class UpdateFinancialRequestHandler : UpdateFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _FinancialService;

    public UpdateFinancialRequestHandler(IMongoService<Domain.Entities.Financial> FinancialService)
    {
        _FinancialService = FinancialService;
    }

    protected override async Task<UpdateFinancialResponse> Handle(UpdateFinancialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _FinancialService.UpdateAsync(request.id, request.Financial);
            return new UpdateFinancialResponse(true, request.Financial);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}