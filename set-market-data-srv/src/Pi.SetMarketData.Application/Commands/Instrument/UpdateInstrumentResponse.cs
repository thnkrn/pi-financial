namespace Pi.SetMarketData.Application.Commands.Instrument;

public record UpdateInstrumentResponse(bool Success, Domain.Entities.Instrument? UpdatedInstrument = null);