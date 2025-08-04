using Pi.SetMarketData.Application.Commands.Financial;
using Pi.SetMarketData.Application.Interfaces.Financial;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Financial;

public class DeleteFinancialRequestHandler : DeleteFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _financialService;

    public DeleteFinancialRequestHandler(IMongoService<Domain.Entities.Financial> financialService)
    {
        _financialService = financialService;
    }

    protected override async Task<DeleteFinancialResponse> Handle(DeleteFinancialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _financialService.DeleteAsync(request.id);
            return new DeleteFinancialResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}