using Pi.MarketData.Application.Commands.Instrument;
using Pi.MarketData.Application.Interfaces.Instrument;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Instrument;

public class CreateInstrumentRequestHandler : CreateInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public CreateInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<CreateInstrumentResponse> Handle(CreateInstrumentRequest request,
        CancellationToken cancellationToken)
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