using Pi.SetMarketData.Application.Commands.Financial;
using Pi.SetMarketData.Application.Interfaces.Financial;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Financial;

public class CreateFinancialRequestHandler : CreateFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _financialService;

    public CreateFinancialRequestHandler(IMongoService<Domain.Entities.Financial> financialService)
    {
        _financialService = financialService;
    }

    protected override async Task<CreateFinancialResponse> Handle(CreateFinancialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _financialService.CreateAsync(request.Financial);
            return new CreateFinancialResponse(true, request.Financial);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}