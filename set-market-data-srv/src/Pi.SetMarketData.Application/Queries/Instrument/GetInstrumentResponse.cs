namespace Pi.SetMarketData.Application.Queries.Instrument;

public record GetInstrumentResponse(List<Domain.Entities.Instrument> Data);