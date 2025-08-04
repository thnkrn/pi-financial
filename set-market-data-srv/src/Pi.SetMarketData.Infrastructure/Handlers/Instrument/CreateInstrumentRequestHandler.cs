using Pi.SetMarketData.Application.Commands.Instrument;
using Pi.SetMarketData.Application.Interfaces.Instrument;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Instrument;

public class CreateInstrumentRequestHandler : CreateInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public CreateInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<CreateInstrumentResponse> Handle(CreateInstrumentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _instrumentService.CreateAsync(request.Instrument);
            return new CreateInstrumentResponse(true, request.Instrument);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}