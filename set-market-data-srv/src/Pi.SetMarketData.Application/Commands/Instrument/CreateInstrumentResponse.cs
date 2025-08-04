namespace Pi.SetMarketData.Application.Commands.Instrument;

public record CreateInstrumentResponse(bool Success, Domain.Entities.Instrument? CreatedInstrument = null);