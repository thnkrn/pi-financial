namespace Pi.SetMarketData.Application.Commands.InstrumentDetail;

public record CreateInstrumentDetailResponse(bool Success, Domain.Entities.InstrumentDetail? CreatedInstrumentDetail = null);