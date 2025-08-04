namespace Pi.SetMarketData.Application.Commands.InstrumentDetail;

public record UpdateInstrumentDetailResponse(bool Success, Domain.Entities.InstrumentDetail? UpdatedInstrumentDetail = null);